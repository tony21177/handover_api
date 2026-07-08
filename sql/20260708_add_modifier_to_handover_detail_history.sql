-- 20260708 handover_detail_history 增加修改者欄位
-- 記錄每筆異動紀錄實際的修改者（登入者），與原始建立者 CreatorId/CreatorName 區分
ALTER TABLE `handover_detail_history`
    ADD COLUMN `ModifierId` VARCHAR(45) NULL AFTER `CreatorName`,
    ADD COLUMN `ModifierName` VARCHAR(45) NULL AFTER `ModifierId`;
