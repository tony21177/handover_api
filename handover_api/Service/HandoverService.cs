using AutoMapper;
using handover_api.Controllers.Request;
using handover_api.Models;
using handover_api.Service.ValueObject;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Transactions;

namespace handover_api.Service
{
    public class HandoverService
    {
        private readonly HandoverContext _dbContext;
        private readonly ILogger<HandoverService> _logger;
        private readonly IMapper _mapper;

        public HandoverService(HandoverContext dbContext, ILogger<HandoverService> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        public List<HandoverSheetMain> GetAllHandoverSheetMain()
        {
            return _dbContext.HandoverSheetMains.ToList();
        }

        public HandoverSheetMain? GetSheetMainByMainSheetId(int mainSheetId)
        {
            return _dbContext.HandoverSheetMains.Where(m=>m.SheetId==mainSheetId).FirstOrDefault();
        }

        public HandoverSheetGroup? GetSheetGroupBySheetGroupId(int sheetGroupId)
        {
            return _dbContext.HandoverSheetGroups.Where(g => g.SheetGroupId == sheetGroupId).FirstOrDefault();
        }

        public List<HandoverSheetGroup> GetSheetGroupByMainSheetId(int mainSheetId)
        {
            return _dbContext.HandoverSheetGroups.Where(g => g.MainSheetId == mainSheetId).ToList();
        }

        public List<HandoverSheetRow> GetSheetRowsByMainSheetIdAndInSheetGroupIds(int mainSheetId, List<int> sheetGroupId)
        {
            return _dbContext.HandoverSheetRows.Where(r=>r.MainSheetId==mainSheetId&&r.SheetGroupId.HasValue &&sheetGroupId.Contains(r.SheetGroupId.Value)).ToList();
        }

        public List<HandoverSheetMain> GetSheetMainListBySheetRowIdList(List<int> sheetRowIdList)
        {
            List<HandoverSheetRow> handoverSheetRows = _dbContext.HandoverSheetRows.Where(row=>sheetRowIdList.Contains(row.SheetRowId)).ToList();

            List<int> mainSheetIdList = handoverSheetRows.Select(row=>row.MainSheetId.Value).ToList() ;

            return _dbContext.HandoverSheetMains.Where(m => mainSheetIdList.Contains(m.SheetId)).ToList();
        }

        public List<HandoverSheetRow> GetSheetRowsByMainSheetId(int mainSheetId)
        {
            return _dbContext.HandoverSheetRows.Where(r => r.MainSheetId==mainSheetId).ToList();
        }


        public List<HandoverSheetMain> UpdateHandoverSheetMains(List<HandoverSheetMain> updateHandoverSheetMainList)
        {
            var updatedHandoverSheetMainList = new List<HandoverSheetMain>();
            updateHandoverSheetMainList.ForEach(updateHandoverSheetMain =>
            {
                var existingSheetMain = _dbContext.HandoverSheetMains.Find(updateHandoverSheetMain.SheetId);
                if (existingSheetMain != null)
                {
                    _mapper.Map(existingSheetMain, updateHandoverSheetMain);
                    _dbContext.Entry(existingSheetMain).CurrentValues.SetValues(updateHandoverSheetMain);
                    updatedHandoverSheetMainList.Add(updateHandoverSheetMain);
                }
            });
            _dbContext.SaveChanges();
            return updatedHandoverSheetMainList;
        }

        public int GetMaxSheetMainId()
        {
            var maxSheetId = _dbContext.HandoverSheetMains
                .Select(sheetMain => (int?)sheetMain.SheetId)
                .OrderByDescending(sheetId => sheetId)
                .FirstOrDefault(); // 取得第一個結果或者 null
            if (maxSheetId.HasValue)
            {
                return maxSheetId.Value; // 如果有值，返回該值
            }
            else
            {
                return 0; // 如果為 null，返回預設值
            }
        }


        public int GetMaxSheetGroupId(int mainSheetId)
        {
            var maxSheetGroupId = _dbContext.HandoverSheetGroups
                .Where(sheetGroup => sheetGroup.MainSheetId == mainSheetId)
                .Select(sheetGroup => (int?)sheetGroup.SheetGroupId) 
                .OrderByDescending(sheetGroupId => sheetGroupId)
                .FirstOrDefault(); // 取得第一個結果或者 null

            if (maxSheetGroupId.HasValue)
            {
                return maxSheetGroupId.Value; // 如果有值，返回該值
            }
            else
            {
                return (int)(mainSheetId * 100); // 如果為 null，返回預設值
            }
        }

        public int GetMaxSheetRowId(int mainSheetId,int sheetGroupId)
        {
            var maxSheetRowId = _dbContext.HandoverSheetRows
                .Where(row => row.MainSheetId==mainSheetId&&row.SheetGroupId==sheetGroupId)
                .Select(row => (int?)row.SheetRowId)
                .OrderByDescending(rowId => rowId)
                .FirstOrDefault(); // 取得第一個結果或者 null

            if (maxSheetRowId.HasValue)
            {
                return maxSheetRowId.Value; // 如果有值，返回該值
            }
            else
            {
                return (int)(sheetGroupId * 1000); // 如果為 null，返回預設值
            }
        }

        public bool CreateHandoverSheetMain(HandoverSheetMain newHandoverSheetMain)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var newId = GetMaxSheetMainId() + 1;
                    newHandoverSheetMain.SheetId = newId;
                    newHandoverSheetMain.Id = Guid.NewGuid().ToString();
                    _dbContext.HandoverSheetMains.Add(newHandoverSheetMain);
                    _dbContext.SaveChanges(true);
                    // 提交事務
                    scope.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    // 處理事務失敗的例外
                    // 這裡可以根據實際需求進行錯誤處理
                    _logger.LogError("事務失敗[CreateHandoverSheetMain]：{msg}", ex.Message);
                    return false;
                }
            }
        }

        public void DeleteHandoverSheetMain(int sheetID)
        {
            var sheetMainToDelete = new HandoverSheetMain { SheetId = sheetID };
            // 將實體的狀態設置為 'Deleted'
            _dbContext.Entry(sheetMainToDelete).State = EntityState.Deleted;

            // 將更改應用到資料庫
            _dbContext.SaveChanges();
            return;
        }

        public void InActiveHandoverSheetMain(int sheetID)
        {
            var updateSheetMain =_dbContext.HandoverSheetMains.Where(m => m.SheetId == sheetID).FirstOrDefault();
            if (updateSheetMain != null)
            {
                updateSheetMain.IsActive = false;
                // 將更改應用到資料庫
                _dbContext.SaveChanges();
            }
            return;
        }

        public List<HandoverSheetGroup> GetAllHandoverSheetGroup()
        {
            return _dbContext.HandoverSheetGroups.ToList();
        }

        public List<HandoverSheetGroup> UpdateHandoverSheetGroups(List<HandoverSheetGroup> updateHandoverSheetGroupList)
        {
            var updatedHandoverSheetGroupList = new List<HandoverSheetGroup>();
            updateHandoverSheetGroupList.ForEach(updateHandoverSheetGroup =>
            {
                var existingSheetGroup = _dbContext.HandoverSheetGroups.Where(group=>group.SheetGroupId==updateHandoverSheetGroup.SheetGroupId).FirstOrDefault();
                if (existingSheetGroup != null)
                {
                    _mapper.Map(existingSheetGroup, updateHandoverSheetGroup);
                    _dbContext.Entry(existingSheetGroup).CurrentValues.SetValues(updateHandoverSheetGroup);
                    updatedHandoverSheetGroupList.Add(updateHandoverSheetGroup);
                }
            });
            // 將變更保存到資料庫
            _dbContext.SaveChanges();
            return updatedHandoverSheetGroupList;
        }

        public bool CreateHandoverSheetGroup(HandoverSheetGroup newHandoverSheetGroup)
        {
            using var scope = new TransactionScope();
            try
            {
                var newSheetGroupId = GetMaxSheetGroupId(newHandoverSheetGroup.MainSheetId.Value);
                newHandoverSheetGroup.SheetGroupId = newSheetGroupId + 1;
                newHandoverSheetGroup.Id = Guid.NewGuid().ToString();
                _dbContext.HandoverSheetGroups.Add(newHandoverSheetGroup);
                _dbContext.SaveChanges(true);
                // 提交事務
                scope.Complete();
                return true;
            }
            catch (Exception ex)
            {
                // 處理事務失敗的例外
                // 這裡可以根據實際需求進行錯誤處理
                _logger.LogError("事務失敗[CreateHandoverSheetGroup]：{msg}", ex.Message);
                return false;
            }
        }

        public void DeleteHandoverSheetGroup(int sheetGroudId)
        {
            var sheetGroupToDelete = new HandoverSheetGroup { SheetGroupId = sheetGroudId };
            // 將實體的狀態設置為 'Deleted'
            _dbContext.Entry(sheetGroupToDelete).State = EntityState.Deleted;

            // 將更改應用到資料庫
            _dbContext.SaveChanges();
            return;
        }

        public void InActiveHandoverSheetGroup(int sheetGroudId)
        {
            var updateSheetMain = _dbContext.HandoverSheetGroups.Where(m => m.SheetGroupId == sheetGroudId).FirstOrDefault();
            if (updateSheetMain != null)
            {
                updateSheetMain.IsActive = false;
                // 將更改應用到資料庫
                _dbContext.SaveChanges();
            }
            
            return;
        }

        public List<HandoverSheetRow> GetAllHandoverSheetRow()
        {
            return _dbContext.HandoverSheetRows.ToList();
        }

        public List<HandoverSheetRow> UpdateHandoverSheetRows(List<HandoverSheetRow> updateHandoverSheetRowList)
        {
            var updatedHandoverSheetRowList = new List<HandoverSheetRow>();
            updateHandoverSheetRowList.ForEach(updateHandoverSheetRow =>
            {
                var existingSheetRow = _dbContext.HandoverSheetRows.Where(r => r.SheetRowId == updateHandoverSheetRow.SheetRowId).FirstOrDefault();
                if (existingSheetRow != null)
                {
                    _mapper.Map(existingSheetRow, updateHandoverSheetRow);
                    // 使用 SetValues 來只更新不為 null 的屬性
                    _dbContext.Entry(existingSheetRow).CurrentValues.SetValues(updateHandoverSheetRow);
                    updatedHandoverSheetRowList.Add(updateHandoverSheetRow);
                }
            });
            // 將變更保存到資料庫
            _dbContext.SaveChanges();
            return updatedHandoverSheetRowList;
        }

        public bool CreateHandoverSheetRow(HandoverSheetRow newHandoverSheetRow)
        {
            using var scope = new TransactionScope();
            try
            {
                var newSheetRowId = GetMaxSheetRowId(newHandoverSheetRow.MainSheetId.Value,newHandoverSheetRow.SheetGroupId.Value);
                newHandoverSheetRow.SheetRowId= newSheetRowId + 1;
                newHandoverSheetRow.Id = Guid.NewGuid().ToString();
                _dbContext.HandoverSheetRows.Add(newHandoverSheetRow);
                _dbContext.SaveChanges(true);
                // 提交事務
                scope.Complete();
                return true;
            }
            catch (Exception ex)
            {
                // 處理事務失敗的例外
                // 這裡可以根據實際需求進行錯誤處理
                _logger.LogError("事務失敗[CreateHandoverSheetRow]：{msg}", ex.Message);
                return false;
            }
            
        }

        public void DeleteHandoverSheetRow(int sheetRowId)
        {
            ;
            var sheetRowToDelete = new HandoverSheetRow { SheetRowId = sheetRowId };
            // 將實體的狀態設置為 'Deleted'
            _dbContext.Entry(sheetRowToDelete).State = EntityState.Deleted;

            // 將更改應用到資料庫
            _dbContext.SaveChanges();
            return;
        }

        public void InActiveHandoverSheetRow(int sheetRowId)
        {
            var updateSheetRow = _dbContext.HandoverSheetRows.Where(m => m.SheetRowId == sheetRowId).FirstOrDefault();
            if (updateSheetRow != null)
            {
                updateSheetRow.IsActive = false;
                // 將更改應用到資料庫
                _dbContext.SaveChanges();
            }

            return;
        }

        public List<SheetSetting> GetAllSettings()
        {
            List<HandoverSheetMain> handoverSheetMainList = GetAllHandoverSheetMain();
            List<SheetSetting> sheetSettingDtoList = _mapper.Map<List<SheetSetting>>(handoverSheetMainList);


            List<HandoverSheetGroup> handoverSheetGroups = GetAllHandoverSheetGroup();
            List<HandoverSheetGroupDto> handoverSheetGroupDtoList = _mapper.Map<List<HandoverSheetGroupDto>>(handoverSheetGroups);
            List<HandoverSheetRow> handoverSheetRows = GetAllHandoverSheetRow();

            handoverSheetGroupDtoList.ForEach(groupDto =>
            {
                List<HandoverSheetRow> matchedHandoverSheetRows = handoverSheetRows.Where(row => row.SheetGroupId == groupDto.SheetGroupId).ToList();
                groupDto.HandoverSheetRowList = matchedHandoverSheetRows;
            });

            sheetSettingDtoList.ForEach(settingDto =>
            {
                List<HandoverSheetGroupDto> matchedSheetGroupDtoList = handoverSheetGroupDtoList.Where(settingGroupDto => settingGroupDto.MainSheetId == settingDto.SheetId).ToList();
                settingDto.HandoverSheetGroupList = matchedSheetGroupDtoList;
            });
            return sheetSettingDtoList;
        }

        // 要保證進來的rowDetails都屬於同一個handoverSheetMain
        public string? CreateHandOverDetail(List<RowDetail> rowDetails,List<Member> readerMemberList,Member creator)
        {
            if (rowDetails.Count == 0) { return null; }

            int mainSheetId = rowDetails[0].HandoverSheetRowSetting.MainSheetId.Value;
            var mainSheetSetting = GetSheetMainByMainSheetId(mainSheetId);
            List<HandoverSheetGroup> handoverSheetGroups = GetSheetGroupByMainSheetId(mainSheetId);
            List<int> inSheetGroupIdList = handoverSheetGroups.Select(group => group.SheetGroupId).ToList();
            List<HandoverSheetRow> handoverSheetRows = GetSheetRowsByMainSheetIdAndInSheetGroupIds(mainSheetId, inSheetGroupIdList);

            HandoverSheetRowDetailAndSettings handoverSheetRowDetailAndSettings = _mapper.Map<HandoverSheetRowDetailAndSettings>(mainSheetSetting);
            List<GroupSetting> groupSettings = _mapper.Map<List<GroupSetting>>(handoverSheetGroups);
            List<RowSettingAndDetail> rowSettingAndDetails = _mapper.Map<List<RowSettingAndDetail>>(handoverSheetRows);

            // 補齊handoverSheetRowDetailAndSettings欄位
            List<Reader> readers = readerMemberList.Select(m =>
            {
                return new Reader
                {
                    UserId = m.UserId,
                    Name = m.DisplayName,
                    IsRead = false
                };
            }).ToList();
            handoverSheetRowDetailAndSettings.readers = readers;
            handoverSheetRowDetailAndSettings.HandoverSheetGroupList = groupSettings;

            // 補齊handoverSheetRowDetailAndSettings.HandoverSheetGroupLis欄位
            handoverSheetRowDetailAndSettings.HandoverSheetGroupList.ForEach(group =>
            {
                // 補齊RowSettingAndDetail欄位
                var matchedRowSettingAndDetailList = rowSettingAndDetails.FindAll(r => r.SheetGroupId == group.SheetGroupId);
                matchedRowSettingAndDetailList.ForEach(row =>
                {
                    var matchedRowDetail = rowDetails.Find(rd => rd.SheetRowId == row.SheetRowId);
                    if (matchedRowDetail != null)
                    {
                        row.Status = matchedRowDetail.Status;
                        row.Comment = matchedRowDetail.Comment;
                    }
                    else
                    {
                        _logger.LogError("[CreateHandOverDetail] 區少row  setting:rowId={rowId}的交班資料", row.SheetRowId);
                    }
                });
                group.RowSettingAndDetailList = matchedRowSettingAndDetailList;
            });

            string jsonContent = JsonSerializer.Serialize(handoverSheetRowDetailAndSettings);

            // 新增handover_detail
            HandoverDetail newHandoverDetail = new HandoverDetail
            {
                HandoverDetailId = Guid.NewGuid().ToString(),
                MainSheetId = mainSheetId,
                JsonContent = jsonContent,
                CreatorId = creator.UserId,
                CreatorName = creator.DisplayName
            };

            

            using var scope = new TransactionScope();
            try
            {
                _dbContext.HandoverDetails.Add(newHandoverDetail);
                List<HandoverDetailReader> handoverDetailReaders =readerMemberList.Select(reader => {
                    HandoverDetailReader handoverReader = new()
                    {
                        HandoverDetailId = newHandoverDetail.HandoverDetailId,
                        UserId = reader.UserId,
                        UserName = reader.DisplayName,
                        IsRead = false,


                    };
                    return handoverReader;
                }).ToList();
                _dbContext.HandoverDetailReaders.AddRange(handoverDetailReaders);
                _dbContext.SaveChanges(true);
                // 提交事務
                scope.Complete();
                return jsonContent;
            }
            catch (Exception ex)
            {
                // 處理事務失敗的例外
                // 這裡可以根據實際需求進行錯誤處理
                _logger.LogError("事務失敗[CreateHandOverDetail]：{msg}", ex.Message);
                return null;
            }

        }
    }
}
