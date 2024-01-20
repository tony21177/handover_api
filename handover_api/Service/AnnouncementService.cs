using handover_api.Models;
using handover_api.Service.ValueObject;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace handover_api.Service
{
    public class AnnouncementService
    {
        private readonly HandoverContext _dbContext;
        private readonly ILogger<AnnouncementService> _logger;

        public AnnouncementService(HandoverContext dbContext, ILogger<AnnouncementService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public List<Announcement> GetAllAnnouncements()
        {
            return _dbContext.Announcements.ToList();
        }

        public Announcement? GetAnnouncementByAnnounceId(string annoucementId)
        {
            return _dbContext.Announcements.Where(annoucement => annoucement.AnnounceId == annoucementId).FirstOrDefault();
        }

        public List<AnnouncementWithAttachmentViewModel> GetAllAnnouncementsWithAttachments()
        {
            var query = from announcement in _dbContext.Announcements
                        join attachment in _dbContext.AnnounceAttachments on announcement.AnnounceId equals attachment.AnnounceId
                        select new AnnouncementWithAttachmentViewModel
                        {
                            // Announcement 屬性
                            Id = announcement.Id,
                            Title = announcement.Title,
                            Content = announcement.Content,
                            BeginPublishTime = announcement.BeginPublishTime,
                            EndPublishTime = announcement.EndPublishTime,
                            BeginViewTime = announcement.BeginViewTime,
                            EndViewTime = announcement.EndViewTime,
                            IsActive = announcement.IsActive,
                            AnnounceId = announcement.AnnounceId,
                            CreatorId = announcement.CreatorId,
                            CreatorName = announcement.CreatorName,
                            CreatedTime = announcement.CreatedTime,
                            UpdatedTime = announcement.UpdatedTime,

                            // AnnounceAttachment 屬性
                            AttId = attachment.AttId,
                            Index = attachment.Index,
                            FileName = attachment.FileName,
                            FilePath = attachment.FilePath,
                            FileType = attachment.FileType,
                            FileSizeText = attachment.FileSizeText,
                            FileSizeNumber = attachment.FileSizeNumber,
                            AttachmentCreatedTime = attachment.CreatedTime,
                            AttachmentUpdatedTime = attachment.UpdatedTime,
                            AttachmentIsActive = attachment.IsActive,
                            AttachmentCreatorId = attachment.CreatorId,
                        };

            List<AnnouncementWithAttachmentViewModel> result = query.ToList();
            return result;
        }

        public Announcement? CreateAnnouncement(Announcement announcement, List<string> readerUserIdList, Member creator, List<string> attIdList)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    // 生成新的 AnnounceId
                    announcement.AnnounceId = Guid.NewGuid().ToString();
                    announcement.CreatorId = creator.UserId; announcement.CreatorName = creator.DisplayName;

                    // 將 Announcement 實例添加到 _dbContext.Announcements 中
                    _dbContext.Announcements.Add(announcement);

                    readerUserIdList.ForEach(
                        userId =>
                        {
                            var newAnnouceReader = new AnnouceReader
                            {
                                ReaderId = Guid.NewGuid().ToString(),
                                AnnounceId = announcement.AnnounceId,
                                UserId = userId, //收件人
                                IsRead = false,
                                IsActive = true,
                            };
                            _dbContext.AnnouceReaders.Add(newAnnouceReader);
                            var myAnnouncemnet = new MyAnnouncement
                            {
                                Title = announcement.Title,
                                Content = announcement.Content,
                                BeginPublishTime = announcement.BeginPublishTime,
                                EndPublishTime = announcement.EndPublishTime,
                                BeginViewTime = announcement.BeginViewTime,
                                EndViewTime = announcement.EndViewTime,
                                IsActive = announcement.IsActive,
                                AnnounceId = announcement.AnnounceId,
                                CreatorId = creator.UserId,
                                UserId = userId, //收件人
                                IsBookToTop = false,
                                IsRemind = false,
                            };
                            _dbContext.MyAnnouncements.Add(myAnnouncemnet);
                        });

                    if (attIdList.Count > 0)
                    {
                        UpdateAnnounceAttachments(attIdList, announcement.AnnounceId, announcement.CreatorId);
                    }
                    // 保存更改到資料庫
                    _dbContext.SaveChanges();

                    // 提交事務
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    // 處理事務失敗的例外
                    // 這裡可以根據實際需求進行錯誤處理
                    _logger.LogError("事務失敗：{msg}", ex.Message);
                    return null;
                }
            }

            // 返回新創建的 Announcement 實例
            return announcement;
        }


        public List<AnnounceAttachment> AnnounceAttachments(List<FileDetail> fileDetails)
        {
            var lastIndex = _dbContext.AnnounceAttachments
                .OrderByDescending(a => a.Index)
                .Select(a => a.Index)
                .FirstOrDefault();

            var newAnnounceAttachments = fileDetails.Select(fileDetail =>
            {
                lastIndex++;
                var announceAttachment = new AnnounceAttachment
                {
                    AttId = Guid.NewGuid().ToString(),
                    Index = lastIndex,
                    FileName = fileDetail.FileName,
                    FilePath = fileDetail.FilePath,
                    FileSizeNumber = fileDetail.FileSizeNumber,
                    FileSizeText = fileDetail.FileSizeText,
                    FileType = fileDetail.FileType,
                };

                _dbContext.AnnounceAttachments.Add(announceAttachment);

                return announceAttachment;
            }).ToList();
            _dbContext.SaveChanges();
            return newAnnounceAttachments;
        }

        public List<AnnounceAttachment> GetAnnounceAttachmentsByAttIds(List<string> attIdList)
        {
            if (attIdList.Count == 0)
            {
                return new List<AnnounceAttachment>();
            }

            return _dbContext.AnnounceAttachments.Where(attachment => attIdList.Contains(attachment.AttId)).ToList();
        }

        public void UpdateAnnounceAttachments(List<string> attIds, string annoucementId, string creatorId)
        {
            // Construct the SQL UPDATE statement
            var updateSql = $@"
            UPDATE announce_attachment
            SET AnnounceId = '{annoucementId}', CreatorId = '{creatorId}'
            WHERE AttId IN ({string.Join(",", attIds.Select(id => $"'{id}'"))})";

            // Execute the raw SQL update statement
            _dbContext.Database.ExecuteSqlRaw(updateSql);
        }
    }
}
