﻿using handover_api.Controllers.Request;
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

        public List<Announcement> GetFilteredAnnouncements(ListAnnoucementRequest request)
        {
            var query = _dbContext.Announcements.AsQueryable();

            // Apply filters based on the request parameters
            if (!string.IsNullOrEmpty(request.CreatorID))
                query = query.Where(a => a.CreatorId == request.CreatorID);

            if (request.IsActive.HasValue)
                query = query.Where(a => a.IsActive == request.IsActive);

            if (!string.IsNullOrEmpty(request.Title))
                query = query.Where(a => a.Title.Contains(request.Title));

            if (!string.IsNullOrEmpty(request.Content))
                query = query.Where(a => a.Content.Contains(request.Content));
            // Ordering
            switch (request.OrderBy)
            {
                case "Title":
                    query = request.IsAsc ? query.OrderBy(a => a.Title) : query.OrderByDescending(a => a.Title);
                    break;
                case "Content":
                    query = request.IsAsc ? query.OrderBy(a => a.Content) : query.OrderByDescending(a => a.Content);
                    break;
                case "BeginPublishTime":
                    query = request.IsAsc ? query.OrderBy(a => a.BeginPublishTime) : query.OrderByDescending(a => a.BeginPublishTime);
                    break;
                case "EndPublishTime":
                    query = request.IsAsc ? query.OrderBy(a => a.EndPublishTime) : query.OrderByDescending(a => a.EndPublishTime);
                    break;
                case "BeginViewTime":
                    query = request.IsAsc ? query.OrderBy(a => a.BeginViewTime) : query.OrderByDescending(a => a.BeginViewTime);
                    break;
                case "EndViewTime":
                    query = request.IsAsc ? query.OrderBy(a => a.EndViewTime) : query.OrderByDescending(a => a.EndViewTime);
                    break;
                case "CreatedTime":
                    query = request.IsAsc ? query.OrderBy(a => a.CreatedTime) : query.OrderByDescending(a => a.CreatedTime);
                    break;
                case "UpdatedTime":
                    query = request.IsAsc ? query.OrderBy(a => a.UpdatedTime) : query.OrderByDescending(a => a.UpdatedTime);
                    break;
                default:
                    // Default ordering by Id
                    query = request.IsAsc ? query.OrderBy(a => a.Id) : query.OrderByDescending(a => a.Id);
                    break;
            }

            // Pagination
            if (request.IsPagination && request.PageIndex.HasValue && request.PageSize.HasValue)
                query = query.Skip((request.PageIndex.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);

            return query.ToList();
        }

        public List<AnnounceAttachment> GetAttachmentsByAnnounceIds(List<string> inAnnounceIds)
        {
            var query = _dbContext.AnnounceAttachments.AsQueryable();

            // Apply filter based on the list of AnnounceIds
            query = query.Where(a => inAnnounceIds.Contains(a.AnnounceId));

            return query.ToList();
        }

        public void DeleteAttachmentByAttIds(List<string> attIdList)
        {
            _dbContext.AnnounceAttachments.Where(attachment=>attIdList.Contains(attachment.AttId)).ExecuteDelete();
            return;
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

        public List<AnnouceReader> GetAnnouceReadersByUserIds(List<string> userIds)
        {
            if (userIds.Count == 0)
            {
                return new List<AnnouceReader>();
            }
            return _dbContext.AnnouceReaders.Where(annouceReader=> userIds.Contains(annouceReader.UserId)).ToList();
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