﻿using AutoMapper;
using handover_api.Models;
using handover_api.Service.ValueObject;
using Microsoft.EntityFrameworkCore;
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
            var maxSheetId = _dbContext.HandoverSheetMains.Max(sheet => sheet.SheetId);
            return maxSheetId;
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

        public List<HandoverSheetGroup> GetAllHandoverSheetGroup()
        {
            return _dbContext.HandoverSheetGroups.ToList();
        }

        public List<HandoverSheetGroup> UpdateHandoverSheetGroups(List<HandoverSheetGroup> updateHandoverSheetGroupList)
        {
            var updatedHandoverSheetGroupList = new List<HandoverSheetGroup>();
            updateHandoverSheetGroupList.ForEach(updateHandoverSheetGroup =>
            {
                var existingSheetGroup = _dbContext.HandoverSheetRows.Find(updateHandoverSheetGroup.SheetGroupId);
                if (existingSheetGroup != null)
                {
                    // 使用 SetValues 來只更新不為 null 的屬性
                    _dbContext.Entry(existingSheetGroup).CurrentValues.SetValues(updateHandoverSheetGroup);
                    updatedHandoverSheetGroupList.Add(updateHandoverSheetGroup);
                }
            });
            // 將變更保存到資料庫
            _dbContext.SaveChanges();
            return updatedHandoverSheetGroupList;
        }

        public HandoverSheetGroup CreateHandoverSheetGroup(HandoverSheetGroup newHandoverSheetGroup)
        {
            _dbContext.HandoverSheetGroups.Add(newHandoverSheetGroup);
            _dbContext.SaveChanges(true);
            return newHandoverSheetGroup;
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

        public List<HandoverSheetRow> GetAllHandoverSheetRow()
        {
            return _dbContext.HandoverSheetRows.ToList();
        }

        public List<HandoverSheetRow> UpdateHandoverSheetRows(List<HandoverSheetRow> updateHandoverSheetRowList)
        {
            var updatedHandoverSheetRowList = new List<HandoverSheetRow>();
            updateHandoverSheetRowList.ForEach(updateHandoverSheetRow =>
            {
                var existingSheetRow = _dbContext.HandoverSheetRows.Find(updateHandoverSheetRow.SheetGroupId);
                if (existingSheetRow != null)
                {
                    // 使用 SetValues 來只更新不為 null 的屬性
                    _dbContext.Entry(existingSheetRow).CurrentValues.SetValues(updateHandoverSheetRow);
                    updatedHandoverSheetRowList.Add(updateHandoverSheetRow);
                }
            });
            // 將變更保存到資料庫
            _dbContext.SaveChanges();
            return updatedHandoverSheetRowList;
        }

        public HandoverSheetRow CreateHandoverSheetRow(HandoverSheetRow newHandoverSheetRow)
        {
            _dbContext.HandoverSheetRows.Add(newHandoverSheetRow);
            _dbContext.SaveChanges(true);
            return newHandoverSheetRow;
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
    }
}