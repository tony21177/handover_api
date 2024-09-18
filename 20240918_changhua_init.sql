-- MySQL dump 10.13  Distrib 8.0.33, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: hand_over
-- ------------------------------------------------------
-- Server version	8.0.39

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `annouce_reader`
--

DROP TABLE IF EXISTS `annouce_reader`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `annouce_reader` (
  `id` int NOT NULL AUTO_INCREMENT,
  `ReaderID` varchar(100) NOT NULL COMMENT 'PK(GUID)',
  `AnnounceID` varchar(100) DEFAULT NULL COMMENT 'announcement.AnnounceID',
  `UserID` varchar(100) DEFAULT NULL COMMENT 'member.UserID,收件人',
  `IsRead` tinyint(1) NOT NULL DEFAULT '0',
  `ReadTime` timestamp NULL DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `ReaderID_UNIQUE` (`ReaderID`)
) ENGINE=InnoDB AUTO_INCREMENT=116 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `annouce_reader`
--

LOCK TABLES `annouce_reader` WRITE;
/*!40000 ALTER TABLE `annouce_reader` DISABLE KEYS */;
/*!40000 ALTER TABLE `annouce_reader` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `announce_attachment`
--

DROP TABLE IF EXISTS `announce_attachment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `announce_attachment` (
  `id` int NOT NULL AUTO_INCREMENT,
  `AttID` varchar(100) NOT NULL,
  `Index` int NOT NULL COMMENT '上傳檔案的順序',
  `FileName` varchar(200) DEFAULT NULL,
  `FilePath` varchar(200) DEFAULT NULL,
  `FileType` varchar(45) DEFAULT NULL,
  `FileSizeText` varchar(45) DEFAULT NULL,
  `FileSizeNumber` double DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `AnnounceID` varchar(100) DEFAULT NULL COMMENT 'announcement.AnnounceID\n',
  `CreatorID` varchar(45) DEFAULT NULL COMMENT 'member.UserID',
  `CreatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `AttID_UNIQUE` (`AttID`),
  UNIQUE KEY `Index_UNIQUE` (`Index`)
) ENGINE=InnoDB AUTO_INCREMENT=100 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='公告附件';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `announce_attachment`
--

LOCK TABLES `announce_attachment` WRITE;
/*!40000 ALTER TABLE `announce_attachment` DISABLE KEYS */;
/*!40000 ALTER TABLE `announce_attachment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `announcement`
--

DROP TABLE IF EXISTS `announcement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `announcement` (
  `id` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(1000) DEFAULT NULL,
  `Content` varchar(2000) DEFAULT NULL,
  `BeginPublishTime` datetime DEFAULT NULL,
  `EndPublishTime` datetime DEFAULT NULL,
  `BeginViewTime` datetime DEFAULT NULL,
  `EndViewTime` datetime DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `AnnounceID` varchar(100) NOT NULL,
  `CreatorID` varchar(100) DEFAULT NULL COMMENT '對準 Member 的 UserID',
  `CreatorName` varchar(100) DEFAULT NULL,
  `CreatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `AnnounceID_UNIQUE` (`AnnounceID`)
) ENGINE=InnoDB AUTO_INCREMENT=56 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='公告';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `announcement`
--

LOCK TABLES `announcement` WRITE;
/*!40000 ALTER TABLE `announcement` DISABLE KEYS */;
/*!40000 ALTER TABLE `announcement` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `announcement_history`
--

DROP TABLE IF EXISTS `announcement_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `announcement_history` (
  `id` int NOT NULL AUTO_INCREMENT,
  `OldTitle` varchar(1000) DEFAULT NULL,
  `NewTitle` varchar(1000) DEFAULT NULL,
  `OldContent` varchar(1000) DEFAULT NULL,
  `NewContent` varchar(2000) DEFAULT NULL,
  `OldBeginPublishTime` datetime DEFAULT NULL,
  `NewBeginPublishTime` datetime DEFAULT NULL,
  `OldEndPublishTime` datetime DEFAULT NULL,
  `NewEndPublishTime` datetime DEFAULT NULL,
  `OldBeginViewTime` datetime DEFAULT NULL,
  `NewBeginViewTime` datetime DEFAULT NULL,
  `OldEndViewTime` datetime DEFAULT NULL,
  `NewEndViewTime` datetime DEFAULT NULL,
  `OldIsActive` tinyint(1) DEFAULT NULL,
  `NewIsActive` tinyint(1) DEFAULT NULL,
  `AnnounceID` varchar(100) NOT NULL,
  `CreatorID` varchar(100) NOT NULL,
  `CreatorName` varchar(100) DEFAULT NULL,
  `UpdatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `OldAttID` varchar(100) DEFAULT NULL,
  `NewAttID` varchar(100) DEFAULT NULL,
  `OldReaderUserIdList` varchar(2000) DEFAULT NULL,
  `NewReaderUserIdList` varchar(2000) DEFAULT NULL,
  `Action` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=62 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='公告更改紀錄';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `announcement_history`
--

LOCK TABLES `announcement_history` WRITE;
/*!40000 ALTER TABLE `announcement_history` DISABLE KEYS */;
/*!40000 ALTER TABLE `announcement_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `authlayer`
--

DROP TABLE IF EXISTS `authlayer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `authlayer` (
  `id` int NOT NULL AUTO_INCREMENT,
  `AuthName` varchar(45) DEFAULT NULL,
  `AuthValue` smallint DEFAULT NULL,
  `IsCreateAnnouce` tinyint(1) NOT NULL,
  `IsUpdateAnnouce` tinyint(1) NOT NULL DEFAULT '0',
  `IsDeleteAnnouce` tinyint(1) NOT NULL DEFAULT '0',
  `IsHideAnnouce` tinyint(1) NOT NULL DEFAULT '0',
  `IsCreateHandover` tinyint(1) NOT NULL DEFAULT '0',
  `IsUpdateHandover` tinyint(1) NOT NULL DEFAULT '0',
  `IsDeleteHandover` tinyint(1) NOT NULL DEFAULT '0',
  `IsMemberControl` tinyint(1) NOT NULL DEFAULT '0',
  `IsCheckReport` tinyint(1) NOT NULL DEFAULT '0',
  `AuthDescription` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `AuthName_UNIQUE` (`AuthName`),
  UNIQUE KEY `AuthValue_UNIQUE` (`AuthValue`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='權限階層 Table';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `authlayer`
--

LOCK TABLES `authlayer` WRITE;
/*!40000 ALTER TABLE `authlayer` DISABLE KEYS */;
INSERT INTO `authlayer` VALUES (11,'最高層級',1,1,1,1,1,1,1,1,1,1,'適用實驗室主管'),(12,'第一層級',3,1,1,1,1,1,1,1,1,1,'適用管理階層'),(13,'第二層級',5,0,1,1,1,1,0,1,0,1,'適用部門主管'),(14,'第三層級',7,0,1,0,0,1,1,0,0,1,'適用一般醫檢師'),(15,'第四層級',9,1,1,0,0,0,0,0,0,0,'適用行政人員');
/*!40000 ALTER TABLE `authlayer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `file_detail_info`
--

DROP TABLE IF EXISTS `file_detail_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `file_detail_info` (
  `id` int NOT NULL AUTO_INCREMENT,
  `AttID` varchar(100) NOT NULL,
  `FileName` varchar(200) DEFAULT NULL,
  `FilePath` varchar(200) DEFAULT NULL,
  `FileType` varchar(45) DEFAULT NULL,
  `FileSizeText` varchar(45) DEFAULT NULL,
  `FileSizeNumber` double DEFAULT NULL,
  `CreatorID` varchar(45) DEFAULT NULL COMMENT 'member.UserID',
  `CreatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `AttID_UNIQUE` (`AttID`)
) ENGINE=InnoDB AUTO_INCREMENT=123 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='上傳檔案';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `file_detail_info`
--

LOCK TABLES `file_detail_info` WRITE;
/*!40000 ALTER TABLE `file_detail_info` DISABLE KEYS */;
/*!40000 ALTER TABLE `file_detail_info` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `handover_detail`
--

DROP TABLE IF EXISTS `handover_detail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `handover_detail` (
  `HandoverDetailId` varchar(100) NOT NULL,
  `Title` varchar(1000) DEFAULT NULL,
  `Content` varchar(2000) DEFAULT NULL,
  `MainSheetID` int NOT NULL,
  `JsonContent` json DEFAULT NULL,
  `CreatorId` varchar(45) DEFAULT NULL,
  `CreatorName` varchar(100) DEFAULT NULL,
  `CreatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `FileAttIds` text,
  PRIMARY KEY (`HandoverDetailId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `handover_detail`
--

LOCK TABLES `handover_detail` WRITE;
/*!40000 ALTER TABLE `handover_detail` DISABLE KEYS */;
/*!40000 ALTER TABLE `handover_detail` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `handover_detail_history`
--

DROP TABLE IF EXISTS `handover_detail_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `handover_detail_history` (
  `id` int NOT NULL AUTO_INCREMENT,
  `HandoverDetailId` varchar(100) NOT NULL,
  `OldTitle` varchar(1000) DEFAULT NULL,
  `NewTitle` varchar(1000) DEFAULT NULL,
  `OldContent` varchar(2000) DEFAULT NULL,
  `NewContent` varchar(2000) DEFAULT NULL,
  `MainSheetID` int NOT NULL,
  `OldJsonContent` json DEFAULT NULL,
  `NewJsonContent` json DEFAULT NULL,
  `CreatorId` varchar(45) DEFAULT NULL,
  `CreatorName` varchar(45) DEFAULT NULL,
  `CreatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `OldReaderUserIds` varchar(1000) DEFAULT NULL,
  `NewReaderUserIds` varchar(1000) DEFAULT NULL,
  `OldReaderUserNames` varchar(2000) DEFAULT NULL,
  `NewReaderUserNames` varchar(2000) DEFAULT NULL,
  `OldFileAttIds` text,
  `NewFileAttIds` text,
  `Action` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `handover_detail_history`
--

LOCK TABLES `handover_detail_history` WRITE;
/*!40000 ALTER TABLE `handover_detail_history` DISABLE KEYS */;
/*!40000 ALTER TABLE `handover_detail_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `handover_detail_readers`
--

DROP TABLE IF EXISTS `handover_detail_readers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `handover_detail_readers` (
  `id` int NOT NULL AUTO_INCREMENT,
  `HandoverDetailId` varchar(100) NOT NULL,
  `UserId` varchar(45) NOT NULL,
  `UserName` varchar(45) NOT NULL,
  `IsRead` tinyint(1) NOT NULL DEFAULT '0',
  `ReadTime` timestamp NULL DEFAULT NULL,
  `CreatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `handover_detail_readers`
--

LOCK TABLES `handover_detail_readers` WRITE;
/*!40000 ALTER TABLE `handover_detail_readers` DISABLE KEYS */;
/*!40000 ALTER TABLE `handover_detail_readers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `handover_sheet_group`
--

DROP TABLE IF EXISTS `handover_sheet_group`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `handover_sheet_group` (
  `id` varchar(100) NOT NULL,
  `MainSheetID` int DEFAULT NULL COMMENT 'handover_sheet_main.SheetID',
  `SheetGroupID` int NOT NULL COMMENT 'GUID',
  `GroupTitle` varchar(100) DEFAULT NULL,
  `GroupDescription` varchar(200) DEFAULT NULL,
  `GroupRank` int NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatorName` varchar(100) DEFAULT NULL,
  `CreatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `SheetGroupID_UNIQUE` (`SheetGroupID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='交班表主表底下組別';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `handover_sheet_group`
--

LOCK TABLES `handover_sheet_group` WRITE;
/*!40000 ALTER TABLE `handover_sheet_group` DISABLE KEYS */;
INSERT INTO `handover_sheet_group` VALUES ('04d0a75f-873a-4bf1-8c8f-56ad2be74b83',1,110,'123','234',102,0,'系統管理員','2024-02-19 07:02:57','2024-02-19 08:35:13'),('0f3f69f4-7990-4dc5-8e3a-3f2c56967d15',1,116,'test1','test1',0,1,'系統管理員','2024-02-21 14:56:30','2024-02-21 14:56:40'),('13b40f6b-683b-441b-82c6-b49a2f82d87e',2,204,'test','test',1,1,'系統管理員','2024-02-21 09:30:52','2024-02-21 09:30:52'),('1ba41a52-d44c-42e6-9241-79d301b69060',2,203,'3','3',2,0,'系統管理員','2024-02-19 07:20:10','2024-02-19 08:51:21'),('28be1bdf-710b-4de3-a585-528891537cc2',1,104,'當班確認','包含例行性檢查及檢體收送',2,1,'系統管理員','2024-02-03 05:37:34','2024-02-03 05:52:06'),('2b92c3a7-e9f3-4895-8596-3deb06123cee',1,101,'血庫','包含例行性查核及血品查核',6,1,'系統管理員','2024-02-03 05:37:34','2024-02-03 05:52:06'),('31ee3893-54ee-4d54-ad03-38817c040c6f',1,103,'生化','包含DXC A,i1000(A),cobas b 221(A),cobas b 222(B)',3,1,'系統管理員','2024-02-03 05:37:34','2024-02-03 05:52:06'),('32e44fe1-0e75-401e-bd19-76aef8931481',1,112,'10456','10456',0,0,'系統管理員','2024-02-19 07:17:06','2024-02-19 08:47:31'),('38ef420f-26b5-4ced-9d49-0d1d5e7145a3',1,113,'a123','a234',104,0,'系統管理員','2024-02-19 07:35:55','2024-02-19 08:47:12'),('5f6b0710-57fc-4573-ac47-0567032c44ff',1,111,'1034','1034',0,0,'系統管理員','2024-02-19 07:03:32','2024-02-19 08:47:34'),('6b55d9f7-cc52-4607-9295-741861738ed8',1,114,'TEST','Test1',8,0,'系統管理員','2024-02-21 07:38:07','2024-02-21 07:38:13'),('730d13e4-727e-4140-b907-ad96b948e0ea',2,201,'1','1',0,1,'系統管理員','2024-02-19 07:17:16','2024-02-19 07:17:16'),('7d63d199-3dd6-45b2-8ebe-121cf1aabde5',1,102,'血液','包含LH785,ESR,EKG',5,1,'系統管理員','2024-02-03 05:37:34','2024-02-03 05:52:06'),('7e1f0474-b000-4827-b49c-e43344ae555b',1,106,'尿液','包含iChem Velocity 主機',7,0,'系統管理員','2024-02-03 05:37:34','2024-02-21 14:55:09'),('d9ebfeec-8b9d-462e-bcbe-e075b3d806a8',2,202,'23','23',0,1,'系統管理員','2024-02-19 07:20:06','2024-02-19 07:20:15'),('db617585-f2ee-4b2f-91c3-6e9a0c449b65',1,115,'1','1',7,0,'系統管理員','2024-02-21 14:55:20','2024-02-21 14:57:23'),('e2a2115e-cb9b-4662-9943-75bd71d42081',9,901,'1','1',0,0,'系統管理員','2024-02-21 13:52:39','2024-02-21 13:52:45'),('f6ceff93-68b4-49a7-9daa-7e8438c90e4d',2,205,'儀器有無保養或叫修','',2,1,'系統管理員','2024-09-13 05:57:37','2024-09-13 05:57:37'),('f84ac66b-4ced-4d51-a9f7-511caa4992f9',1,107,'機台','檢驗室機台確認',0,1,'系統管理員','2024-02-03 05:37:34','2024-02-19 03:56:54'),('fbbb9f54-99ca-4625-8455-91aca7fa1860',3,301,'血庫','血庫類別交班表',0,1,'系統管理員','2024-02-19 09:09:57','2024-02-19 09:09:57'),('fbbb9f54-99ca-4625-8455-91aca7fa1862',3,302,'血庫2','血庫類別交班表2',1,0,'系統管理員','2024-02-19 09:09:57','2024-02-19 09:09:57'),('fd15442f-cbc0-442b-8a52-d5d79c078fa0',1,105,'微生物','包含Compact',4,1,'系統管理員','2024-02-03 05:37:34','2024-02-03 05:52:06');
/*!40000 ALTER TABLE `handover_sheet_group` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `handover_sheet_main`
--

DROP TABLE IF EXISTS `handover_sheet_main`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `handover_sheet_main` (
  `SheetID` int NOT NULL,
  `id` varchar(100) NOT NULL,
  `Title` varchar(100) DEFAULT NULL,
  `Description` varchar(100) DEFAULT NULL,
  `Image` varchar(2000) DEFAULT NULL,
  `UpdatedTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `ModifiedOn` timestamp NULL DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `Version` varchar(45) DEFAULT NULL,
  `SerialCode` varchar(100) DEFAULT NULL,
  `CreatorName` varchar(100) DEFAULT NULL,
  `CreatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`SheetID`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='交班表設定主表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `handover_sheet_main`
--

LOCK TABLES `handover_sheet_main` WRITE;
/*!40000 ALTER TABLE `handover_sheet_main` DISABLE KEYS */;
INSERT INTO `handover_sheet_main` VALUES (1,'f30c8962-fb43-4632-b414-8da6a39183b3','中興院區','測試交班Main Title描述','https://fastly.picsum.photos/id/50/600/600.jpg?hmac=Y7JK0JjvBbHSPShLsMmFrDzu1wX1mKrz517-9Sey9No','2024-02-26 07:07:32',NULL,1,'1.0.14','R-QP04011-009','系統管理員','2024-02-03 02:27:41'),(2,'005f0952-9a31-4dba-ac7c-28c6122719f9','血庫','彰化醫院醫事檢驗科血庫交班表',NULL,'2024-09-18 11:36:14',NULL,1,'1.0.13','R-QP04011-009','系統管理員','2024-02-03 02:27:41'),(3,'9398e404-07df-467a-b9b0-f1142790e387','小夜班','彰化醫院醫事檢驗科小夜班交班表',NULL,'2024-09-18 11:36:14',NULL,1,'21.1','R-QP04011-003','系統管理員','2024-02-03 02:27:41'),(4,'72499b08-d033-421c-8059-c5323e455515','外送及抽血櫃台','彰化醫院醫事檢驗科外送及抽血櫃台交班表',NULL,'2024-09-18 11:36:14',NULL,1,'21.3','R-QP04011-004','系統管理員','2024-02-03 02:27:41'),(5,'8efda967-b5fa-4b87-b206-4f089858bd65','生化','彰化醫院醫事檢驗科生化交班表',NULL,'2024-09-18 11:36:14',NULL,1,'21.2','R-QP04011-005','系統管理員','2024-02-03 02:27:41'),(6,'37087271-7e60-4f5b-a291-dc88ddb315b2','假日班','彰化醫院醫事檢驗科假日班表',NULL,'2024-09-18 11:36:14',NULL,1,'21.5','R-QP04011-006','系統管理員','2024-02-03 02:27:41'),(7,'42c817ce-10b2-4aa5-8182-bf5e9b3017df','大夜班','彰化醫院醫事檢驗科大夜班表',NULL,'2024-09-18 11:36:14',NULL,1,'21.5','R-QP04011-007','系統管理員','2024-02-03 02:27:41'),(8,'563b998e-b5ba-4f3c-80d7-948ccb710ba4','鏡檢及血液','彰化醫院醫事檢驗科鏡檢及血液班表',NULL,'2024-09-18 11:36:14',NULL,1,'21.5','R-QP04011-008','系統管理員','2024-02-03 02:27:41'),(9,'11078b0f-28d3-4c36-9ac2-039b5c8ef11a','測試修改交班','修改描述Group','https://fastly.picsum.photos/id/50/600/600.jpg?hmac=Y7JK0JjvBbHSPShLsMmFrDzu1wX1mKrz517-9Sey9No','2024-02-21 13:52:26',NULL,0,'1.0.14','R-QP04011-010','管理員','2024-02-04 06:21:45');
/*!40000 ALTER TABLE `handover_sheet_main` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `handover_sheet_row`
--

DROP TABLE IF EXISTS `handover_sheet_row`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `handover_sheet_row` (
  `id` varchar(100) NOT NULL,
  `MainSheetID` int DEFAULT NULL COMMENT 'handover_sheet_main.SheetID',
  `SheetGroupID` int DEFAULT NULL COMMENT 'handover_sheet_group.SheetGroupID',
  `SheetRowID` int NOT NULL,
  `WeekDays` varchar(100) DEFAULT NULL COMMENT '星期幾要做這個項目',
  `SheetGroupTitle` varchar(100) DEFAULT NULL,
  `RowCategory` varchar(45) DEFAULT NULL,
  `MachineBrand` varchar(100) DEFAULT NULL,
  `MachineCode` varchar(100) DEFAULT NULL,
  `MachineSpec` varchar(200) DEFAULT NULL,
  `MaintainItemName` varchar(100) DEFAULT NULL,
  `MaintainItemDescription` varchar(200) DEFAULT NULL,
  `MaintainItemType` varchar(100) DEFAULT NULL,
  `MaintainAnswerType` varchar(100) DEFAULT NULL,
  `Remarks` varchar(1000) DEFAULT NULL,
  `CreatorName` varchar(100) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `SheetRowID_UNIQUE` (`SheetRowID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='交班表組別底下的Row資料';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `handover_sheet_row`
--

LOCK TABLES `handover_sheet_row` WRITE;
/*!40000 ALTER TABLE `handover_sheet_row` DISABLE KEYS */;
INSERT INTO `handover_sheet_row` VALUES ('3fd1c438-d4cb-4826-ac9d-a3a40df2d68a',1,106,106011,'1,2,3','尿液','aa','ff','','ee','bb','cc','1','1','gg','系統管理員',0,'2024-02-21 07:17:15','2024-02-21 13:40:31'),('43672f23-bb49-4d09-8ddb-d3f52c37603c',1,106,106004,'1,2,3,4,5,6,7','尿液','111123','666678','444456','555567','222234','333345','1','1','777789','系統管理員',0,'2024-02-20 04:04:48','2024-02-20 09:41:13'),('4b8eb603-f3e1-4031-bb2d-459efd00e203',2,202,202002,'4','23','d','d','d','d','d','d','1','1','d','系統管理員',0,'2024-02-21 12:22:40','2024-02-21 13:42:22'),('4cb0630d-fc71-432d-9dba-f1987a1b1b13',1,102,102001,'1,2,3,4,5','血液','Category A','Brand X','ABC123','Specs...','Item 1','Description of Item 1','Type A','Type X','Some remarks','系統管理員',1,'2024-02-17 07:13:57','2024-02-17 07:13:57'),('70a85c00-0eea-413a-8d4e-ab7cec27e511',1,101,101002,'1,2','血庫','1','1','1','1','1','1','1','1','1','系統管理員',1,'2024-09-13 06:46:54','2024-09-13 06:46:54'),('72c3eea4-b857-44c3-8968-2557609df5cb',1,106,106003,'1','尿液','1','6','4','5','2','3','1','1','7','系統管理員',0,'2024-02-20 04:02:23','2024-02-20 04:05:36'),('83cc0227-66d7-4a6d-88d7-75d964e82cad',1,107,107001,'4','機台','c','c','c','c','c','c','1','1','c','系統管理員',1,'2024-02-21 14:53:40','2024-02-21 14:53:40'),('84aba71b-4781-4890-94fa-6f01eb62341d',1,102,102002,'1,2,3,4','血液','Category...','Brand...','ABC123...','SpecsXXX','Item 2','Description of Item 2','Type B','Type C','Some remarks','系統管理員',1,'2024-02-17 07:13:58','2024-02-17 07:27:32'),('8593f818-27b8-4680-8adf-4b36668e5929',1,116,116001,'1,3,5','test1','a','a','a','a','a','a','1','1','a','系統管理員',1,'2024-02-21 14:57:00','2024-02-21 14:57:00'),('9f2874c6-0e51-485e-92fc-139eb5eece9e',1,106,106005,'1,2,3,4,5,6,7','尿液','1','6','4','5','2','3','1','1','7','系統管理員',0,'2024-02-20 09:17:48','2024-02-20 09:41:09'),('a0f02a19-7cc2-4a4f-ae42-ac22fc18100d',1,115,115001,'1,3,5','1','ab','a','a','a','ab','ab','1','1','a','系統管理員',0,'2024-02-21 14:55:34','2024-02-21 14:57:17'),('a39572e1-c5da-4731-b58d-e94f68674125',1,106,106009,'1,2,3','尿液','1','6','4','5','2','3','1','1','7','系統管理員',0,'2024-02-21 06:15:28','2024-02-21 13:40:56'),('a4caf2c6-fcd7-4b59-8856-b3ba62bce7c5',1,106,106002,'1,2,3,4,5','尿液','9','4','6','5','8','7','1','1','3','系統管理員',0,'2024-02-19 16:21:06','2024-02-20 03:09:18'),('a93b93a4-8e5c-4017-9d77-fe07667950e1',1,106,106007,'1,2,3,4,5,6,7','尿液','1','6','4','5','2','3','1','1','7','系統管理員',0,'2024-02-20 09:40:39','2024-02-20 09:41:05'),('ad9923dc-4e6e-44e2-a82f-3c588a1b1bb0',2,201,201002,'1','1','b','b','b','b','b','b','1','1','b','系統管理員',0,'2024-02-21 10:36:17','2024-02-21 13:42:16'),('aed2212a-5551-4aea-a6f3-8edd0f606e83',1,102,102003,'1,2,3,4,5','血液','Category A','Brand X','ABC123','Specs...','Item 1','Description of Item 1','Type A','Type X','Some remarks','系統管理員',1,'2024-02-19 16:02:58','2024-02-19 16:02:58'),('af11f2cc-ee3e-4bdf-a172-5533d9afb8ca',1,106,106008,'1','尿液','1','6','4','5','2','3','1','1','7','系統管理員',1,'2024-02-20 09:41:20','2024-02-20 09:41:20'),('af27fbf8-f87a-4beb-a720-cdb0c8f1797e',3,301,301001,'2','血庫','a','a','a','a','a','a','1','1','a','系統管理員',1,'2024-02-21 10:38:51','2024-02-21 10:38:51'),('af27fbf8-f87a-4beb-a720-cdb0c8f1797f',3,302,302001,'2','血庫2','b','b','b','b','b','b','3','3','b','系統管理員',0,'2024-02-21 10:38:51','2024-02-21 10:38:51'),('af27fbf8-f87a-4beb-a720-cdb0c8f1797g',3,301,301002,'3','血庫1-1','c','c','c','c','c','c','2','2','c','系統管理員',1,'2024-02-21 10:38:51','2024-02-21 10:38:51'),('b49bc8fc-613a-4908-a3a4-e8218c80ab39',1,101,101001,'1,3,5','血液','LH786','BECKMAN COULTER','','','確認開機後儀器內溫度介於70-80℉','(器差值:-0.66℉)允收值69.34-79.64℉','1','1','Some remarks','系統管理員',1,'2024-02-03 06:34:00','2024-02-20 08:17:45'),('b97e1252-7bb7-42f5-bd08-80c1da908919',1,106,106001,'1,2,3,4,5','尿液','','','','','','','1','1','','系統管理員',0,'2024-02-19 16:16:45','2024-02-20 02:36:30'),('bd2524bf-dad0-4701-a430-fa77622a9cd2',2,202,202001,'3','23','c','c','c','c','c','c','1','1','c','系統管理員',0,'2024-02-21 10:37:46','2024-02-21 13:42:26'),('c1dbd067-94c3-4e3b-8c7e-62a9dd902d3b',2,201,201001,'4','1','a','b','b','b','b','b','1','1','b','系統管理員',0,'2024-02-21 10:35:41','2024-02-21 13:42:19'),('d08ccbb2-2762-42c5-a561-d0710cf75561',2,204,204001,'1,2,3','test','1','6','4','5','2','333','1','1','7','系統管理員',1,'2024-02-21 09:31:04','2024-02-21 10:24:31'),('d30f1d09-9df9-4b1f-9860-6e61a71e0cb7',1,106,106006,'1,2,3,4,5,6,7','尿液','1','6','4','5','2','3','1','1','7','系統管理員',0,'2024-02-20 09:40:17','2024-02-20 09:41:00'),('e173c651-2287-44d4-a6af-669adb1ac1e0',1,106,106012,'1,2,3,4','尿液','a','b','b','b','b','b','1','1','b','系統管理員',1,'2024-02-21 14:53:25','2024-02-21 14:53:25'),('e2955375-5ba5-4c55-8657-54bd6c6e68e6',1,106,106010,'1,2,3,4','尿液','a1','f1','d1','e1','b1','c1','1','1','g1','系統管理員',1,'2024-02-21 06:16:06','2024-02-21 07:23:10'),('ea656e30-e1d5-403b-bbfd-6ce9e7e510a5',1,101,101003,'1','血庫','項目名稱','廠牌','編號','規格','維護項目名稱','描述','1','1','備註','系統管理員',1,'2024-09-13 06:50:31','2024-09-13 06:50:31'),('fe8cc518-93b1-4ba6-8592-b1b89348e569',2,204,204002,'7','test','a','f','d','e','b','c','1','1','g','系統管理員',0,'2024-02-21 10:24:52','2024-02-21 13:40:01');
/*!40000 ALTER TABLE `handover_sheet_row` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `member`
--

DROP TABLE IF EXISTS `member`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `member` (
  `id` int NOT NULL AUTO_INCREMENT,
  `Account` varchar(45) NOT NULL,
  `Password` varchar(45) DEFAULT NULL,
  `DisplayName` varchar(45) DEFAULT NULL,
  `UserID` varchar(45) NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `AuthValue` smallint NOT NULL,
  `PhotoURL` varchar(1000) DEFAULT NULL,
  `UID` varchar(100) DEFAULT NULL COMMENT '人員獨立代碼',
  `CreatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `UserID_UNIQUE` (`UserID`),
  UNIQUE KEY `Account_UNIQUE` (`Account`),
  UNIQUE KEY `UID_UNIQUE` (`UID`)
) ENGINE=InnoDB AUTO_INCREMENT=130 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='人員設定\n';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `member`
--

LOCK TABLES `member` WRITE;
/*!40000 ALTER TABLE `member` DISABLE KEYS */;
INSERT INTO `member` VALUES (4,'admin','admin','系統管理員','323445dsaf',1,1,NULL,'admin','2024-01-18 07:09:37','2024-02-07 06:05:48'),(10,'RD@email.com','123456','林永裕','63c0dbc4-fdca-47e8-b084-5880fb596de0',0,1,'https://images.unsplash.com/photo-1544723795-3fb6469f5b39?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w0NTYyMDF8MHwxfHNlYXJjaHwxN3x8bWVtYmVyfGVufDB8fHx8MTcwMjE3NTYwM3ww&ixlib=rb-4.0.3&q=80&w=400','TESTU1','2024-01-31 06:10:42','2024-02-02 02:18:09'),(11,'RD2@email.com','123456','Gary2','cbe8404e-ab79-4698-9111-49a059fb8a5d',1,9,'https://images.unsplash.com/photo-1544723795-3fb6469f5b39?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w0NTYyMDF8MHwxfHNlYXJjaHwxN3x8bWVtYmVyfGVufDB8fHx8MTcwMjE3NTYwM3ww&ixlib=rb-4.0.3&q=80&w=400','TESTU2','2024-01-31 06:27:32','2024-02-07 09:48:26'),(12,'RD3@email.com','123456','RD3','835fe34c-8be7-4e84-9ac1-d5f5c3679a13',1,5,'https://images.unsplash.com/photo-1544723795-3fb6469f5b39?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w0NTYyMDF8MHwxfHNlYXJjaHwxN3x8bWVtYmVyfGVufDB8fHx8MTcwMjE3NTYwM3ww&ixlib=rb-4.0.3&q=80&w=400','TESTU3','2024-01-31 07:09:32','2024-02-02 02:32:11'),(30,'RD4@email.com','12345','RD4','d4867fa5-7e1f-4f3b-b44c-b9d043670ea8',1,5,'https://images.unsplash.com/photo-1544723795-3fb6469f5b39?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w0NTYyMDF8MHwxfHNlYXJjaHwxN3x8bWVtYmVyfGVufDB8fHx8MTcwMjE3NTYwM3ww&ixlib=rb-4.0.3&q=80&w=400','TESTU4','2024-02-02 03:22:49','2024-02-02 03:22:49'),(33,'05312','05312','楊宗翰','05d269c7-6211-4f7c-960e-d2f12b17d130',1,5,NULL,'05312','2024-09-18 11:33:36','2024-09-18 11:33:36'),(34,'03819','03819','李婉君','0ad57d04-1f5d-46b5-8953-088ce0ce8c33',1,3,NULL,'03819','2024-09-18 11:33:36','2024-09-18 11:33:36'),(35,'Amigo','10011','吳佩珊','1301b83d-2484-499d-a24d-96bc6d615fb7',1,1,NULL,'Amigo','2024-09-18 11:33:36','2024-09-18 11:33:36'),(36,'06361','06361','洪名邑','13c790c9-f157-407b-a0b4-29004f72909a',1,5,NULL,'06361','2024-09-18 11:33:36','2024-09-18 11:33:36'),(37,'N3340','N3340','1ICU','14d10cbd-c597-4c8e-8178-903f3b89c3cf',1,5,'NULL','N3340','2024-09-18 11:33:36','2024-09-18 11:33:36'),(38,'05923','05923','楊淨淳','17b230de-c45f-4c7e-8162-9b940d26a835',1,3,NULL,'05923','2024-09-18 11:33:36','2024-09-18 11:33:36'),(39,'N3220','N3220','急診室','186650c7-3077-4e83-adbd-f43740b4e12a',1,5,'NULL','N3220','2024-09-18 11:33:36','2024-09-18 11:33:36'),(40,'qwe12348200@gmail.com','05672','周靜語','1bd7c460-d6aa-4a16-b210-d6e9189d1f6e',1,5,NULL,'qwe12348200@gmail.com','2024-09-18 11:33:36','2024-09-18 11:33:36'),(41,'3848','3848','劉育瑩','1c33d425-8831-4e59-b1e7-1e99f2950e8c',1,3,'NULL','3848','2024-09-18 11:33:36','2024-09-18 11:33:36'),(42,'N2330','N2330','開刀房','1f1b83cd-63e3-4ad6-a636-902d54f5517e',1,5,'NULL','N2330','2024-09-18 11:33:36','2024-09-18 11:33:36'),(43,'2117','2117','廖秋斐','20995390-c3eb-4a5c-b668-ca3850affada',1,3,'NULL','2117','2024-09-18 11:33:36','2024-09-18 11:33:36'),(44,'2873','2873','吳盺玥','21991c06-c221-4ac0-b9d0-001d0bca737e',1,3,'NULL','2873','2024-09-18 11:33:36','2024-09-18 11:33:36'),(45,'4058','4058','林倩如','2416d908-6bee-48fc-b09d-d5b842194f69',1,5,'NULL','4058','2024-09-18 11:33:36','2024-09-18 11:33:36'),(46,'01010','01010','楊珠玲','260ec6d4-a414-4410-85af-b79d3e892430',1,5,NULL,'01010','2024-09-18 11:33:36','2024-09-18 11:33:36'),(47,'06230','06230','詹尉辰','275dd72e-6504-4d78-b588-b055019d6a8b',1,5,NULL,'06230','2024-09-18 11:33:36','2024-09-18 11:33:36'),(48,'3521','3521','邱惠梅','289d30b8-d35a-45bb-9da5-3674f66eca08',1,3,'NULL','3521','2024-09-18 11:33:36','2024-09-18 11:33:36'),(49,'N4340','N4340','7C病房','2f037251-b0ce-47ee-be12-70584005d526',1,5,'NULL','N4340','2024-09-18 11:33:36','2024-09-18 11:33:36'),(50,'03451','03451','賴秋君','3698b66d-1934-44cf-8a19-5f5af814fa79',1,7,NULL,'03451','2024-09-18 11:33:36','2024-09-18 11:33:36'),(51,'5535','5535','陳靖雯','39df5e8b-6765-4fb4-9a01-65cf62bbb256',1,3,'NULL','5535','2024-09-18 11:33:36','2024-09-18 11:33:36'),(52,'2225','2225','何淑貞','469ab4de-52dd-40f4-a274-3a7dcdc4c213',1,3,'NULL','2225','2024-09-18 11:33:36','2024-09-18 11:33:36'),(53,'5661','5661','江夏鋅','4739ab76-9919-4bad-9e3c-a32c9c09daa1',1,3,'NULL','5661','2024-09-18 11:33:36','2024-09-18 11:33:36'),(54,'jenniferlee','21013','李琇琴','4be7b969-70e5-4edf-946f-b0ad3745ffe6',1,1,NULL,'jenniferlee','2024-09-18 11:33:36','2024-09-18 11:33:36'),(55,'N22B0','N22B0','麻醉科','4c34ee79-07d6-4a4b-9152-57e4cb80c385',1,5,'NULL','N22B0','2024-09-18 11:33:36','2024-09-18 11:33:36'),(56,'05662','05662','林晏琪','4ec74284-8aa6-47ba-805a-0b07ec2b9326',1,3,NULL,'05662','2024-09-18 11:33:36','2024-09-18 11:33:36'),(57,'06335','06335','王姵晴','54cc9412-7821-471c-90a4-e82b2c1c94ef',1,5,NULL,'06335','2024-09-18 11:33:36','2024-09-18 11:33:36'),(58,'CH6B01','CH6B01','金萬林管理員','592b8429-3e9e-4dbe-acb0-bebd57ee1229',1,1,'NULL','CH6B01','2024-09-18 11:33:36','2024-09-18 11:33:36'),(59,'N2280','N2280','耳鼻喉科','5d9441ce-b9c9-4d0d-b57a-0eb77a19d479',1,3,'NULL','N2280','2024-09-18 11:33:36','2024-09-18 11:33:36'),(60,'06279','06279','蔡洧勝','5fd39cba-ea3a-4b44-bce0-ec27552b9068',1,5,NULL,'06279','2024-09-18 11:33:36','2024-09-18 11:33:36'),(61,'suderAdmin','suderAdmin','金萬林管理員','60200ec3-df62-437f-8d90-e01ef8bd9dc0',1,1,NULL,'suderAdmin','2024-09-18 11:33:36','2024-09-18 11:33:36'),(62,'N4320','N4320','5C病房','62de1596-4190-4c02-9d71-a7c610817ab1',1,5,'NULL','N4320','2024-09-18 11:33:36','2024-09-18 11:33:36'),(63,'4142','4142','黃聖剛','655a6c99-bbae-48c1-8215-cdaf3ad1f56e',1,3,'NULL','4142','2024-09-18 11:33:36','2024-09-18 11:33:36'),(64,'4846','4846','王憶芬','69fa81f2-18a9-4ef6-9af4-5fe200da81d2',1,3,'NULL','4846','2024-09-18 11:33:36','2024-09-18 11:33:36'),(65,'N3330','N3330','8C病房','6c73cbb5-b103-4024-b841-3bda46298c66',1,5,'NULL','N3330','2024-09-18 11:33:36','2024-09-18 11:33:36'),(66,'04624','04624','陳美君','6e2d82b5-f442-4afc-89b5-363e4b03841d',1,5,NULL,'04624','2024-09-18 11:33:36','2024-09-18 11:33:36'),(67,'3484','3484','朱凰瑛','72a380c2-e7b3-4017-ad67-0602a41cb34e',1,3,'NULL','3484','2024-09-18 11:33:36','2024-09-18 11:33:36'),(68,'04160','04160','潘郁蒨','73fdd7f3-db94-4b2c-a158-99fbbb995da6',1,5,NULL,'04160','2024-09-18 11:33:36','2024-09-18 11:33:36'),(69,'03441','03441','詹仁士','75d92b35-bf02-4151-bfe8-a09eafb64fd8',1,5,NULL,'03441','2024-09-18 11:33:36','2024-09-18 11:33:36'),(70,'2360','2360','謝采宴','78d8e1ae-3f99-4bae-ad89-0e271b10ac38',1,3,'NULL','2360','2024-09-18 11:33:36','2024-09-18 11:33:36'),(71,'N1221','N1221','胃鏡室','7b9ceb6e-bbb2-49ef-b674-287f20875f0b',1,5,'NULL','N1221','2024-09-18 11:33:36','2024-09-18 11:33:36'),(72,'03702','03702','吳進發','7ee6cfd8-d851-4af4-b2f5-0b6cda57a023',1,5,NULL,'03702','2024-09-18 11:33:36','2024-09-18 11:33:36'),(73,'03309','03309','洪儀君','7f7b019d-4cd2-4831-87aa-9d08a30352a2',1,7,NULL,'03309','2024-09-18 11:33:36','2024-09-18 11:33:36'),(74,'N4350','N4350','2C病房','7ff40596-8e4c-4f62-87fd-4ebcea5a0971',1,5,'NULL','N4350','2024-09-18 11:33:36','2024-09-18 11:33:36'),(75,'CHIN01','CHIN01','金萬林管理員','812624ca-8593-496e-95cc-c2be87d97e65',1,3,'NULL','CHIN01','2024-09-18 11:33:36','2024-09-18 11:33:36'),(76,'03762','003762','游雅言','833d1860-9c47-44c2-8252-fcb2c7f81490',1,3,NULL,'03762','2024-09-18 11:33:36','2024-09-18 11:33:36'),(77,'N3320','N3320','RCW','84befa8b-2eea-4fb8-ba25-eeafeaffa63d',1,5,'NULL','N3320','2024-09-18 11:33:36','2024-09-18 11:33:36'),(78,'2477','2477','王羿雯','8613d020-0968-4d54-bb33-ae0a78ea35e1',1,3,'NULL','2477','2024-09-18 11:33:36','2024-09-18 11:33:36'),(79,'N12DO','N12DO','肺功能室','88942dce-063f-4f49-a44d-bccb5aa32f9f',1,5,'NULL','N12DO','2024-09-18 11:33:36','2024-09-18 11:33:36'),(80,'N2230','N2230','神外門診','8b1b51bf-4066-4df0-a9c0-6dad61aa084c',1,5,'NULL','N2230','2024-09-18 11:33:36','2024-09-18 11:33:36'),(81,'3239','3239','吳慧萍','8b999484-30ff-475a-92fe-631cf96c46c6',1,3,'NULL','3239','2024-09-18 11:33:36','2024-09-18 11:33:36'),(82,'04126','04126','郭奕男','8bdfe06e-e473-4a2d-a07a-c521e68eef31',1,5,NULL,'04126','2024-09-18 11:33:36','2024-09-18 11:33:36'),(83,'N5600','N5600','7A病房','8f08cb05-e861-4fd7-a84e-50add7d67e8e',1,5,'NULL','N5600','2024-09-18 11:33:36','2024-09-18 11:33:36'),(84,'2127','2127','柯詒菁','8f7cb832-b743-419b-9b16-b6626205b97b',1,3,'NULL','2127','2024-09-18 11:33:36','2024-09-18 11:33:36'),(85,'2130','2130','金椿期','8fcef85c-2874-4576-b25c-b608b97e7dcd',1,3,'NULL','2130','2024-09-18 11:33:36','2024-09-18 11:33:36'),(86,'N2310','N2310','6A病房','904030db-e029-4f2d-9c06-6f5fffd52dab',1,5,'NULL','N2310','2024-09-18 11:33:36','2024-09-18 11:33:36'),(87,'N4360','N4360','8D護理站','941ba42b-4738-49cb-989a-9a710c64cd4c',1,5,'NULL','N4360','2024-09-18 11:33:36','2024-09-18 11:33:36'),(88,'2398','2398','張佩茹','958cbcc3-c4a2-4cba-afbc-ced4d580b1d0',1,3,'NULL','2398','2024-09-18 11:33:36','2024-09-18 11:33:36'),(89,'N1320','N1320','7B病房','985ea7b6-740e-458e-8d24-10af98bd7363',1,5,'NULL','N1320','2024-09-18 11:33:36','2024-09-18 11:33:36'),(90,'3820','3820','楊雪凰','9a902c3e-4b0c-416c-9c37-5eeebb531844',1,3,'NULL','3820','2024-09-18 11:33:36','2024-09-18 11:33:36'),(91,'04938','04938','孫怡惠','a3b3f066-898c-41f8-b56c-ca1ea2aa6d5b',1,3,NULL,'04938','2024-09-18 11:33:36','2024-09-18 11:33:36'),(92,'2869','2869','陳芸瑩','a7305d13-864b-49e6-9ba1-2f233fd2b468',1,3,'NULL','2869','2024-09-18 11:33:36','2024-09-18 11:33:36'),(93,'N4310','N4310','3C病房','aa094938-1a97-4db1-bf6b-1d3444914590',1,5,'NULL','N4310','2024-09-18 11:33:36','2024-09-18 11:33:36'),(94,'4658','4658','顧婉鈴','aaf6d1d3-6de9-44ef-b8e4-ae9bc16445a1',1,3,'NULL','4658','2024-09-18 11:33:36','2024-09-18 11:33:36'),(95,'CH7C01','CH7C01','金萬林管理員','acbd87c9-cd91-7d4a-b9cf-5a4d348bb131',1,1,'NULL','CH7C01','2024-09-18 11:33:36','2024-09-18 11:33:36'),(96,'03819-04','03819-04','李婉君(代)','aee93a86-b1f8-4005-b61a-2d6f4f3c6b5e',1,3,'NULL','03819-04','2024-09-18 11:33:36','2024-09-18 11:33:36'),(97,'05477','05477','張羽婷','afa05992-ef74-4db1-9160-e4adb0d6d126',1,5,NULL,'05477','2024-09-18 11:33:36','2024-09-18 11:33:36'),(98,'N4330','N4330','6C病房','b035e312-d889-4c3f-9741-523ba04d5f2f',1,5,'NULL','N4330','2024-09-18 11:33:36','2024-09-18 11:33:36'),(99,'N1231','N1231','HDR','b0b86be5-a794-4531-8e7a-6265642f229b',1,5,'NULL','N1231','2024-09-18 11:33:36','2024-09-18 11:33:36'),(100,'05780','05780','張雅宜','b3bc4dd4-80bf-4e7d-b57e-00001146789b',1,5,NULL,'05780','2024-09-18 11:33:36','2024-09-18 11:33:36'),(101,'05065','05065','蔡易昌','b8976357-9f92-4ff0-8318-d6af86c782df',1,7,NULL,'05065','2024-09-18 11:33:36','2024-09-18 11:33:36'),(102,'N2290','N2290','婦產科門診','bd041681-7dab-4073-8e80-0e2bf9183bab',1,5,'NULL','N2290','2024-09-18 11:33:36','2024-09-18 11:33:36'),(103,'04833','04833','洪經緯','c00ff3f1-9551-4d61-a329-ea28c04f6717',1,5,NULL,'04833','2024-09-18 11:33:36','2024-09-18 11:33:36'),(104,'2770','2770','趙珮玉','c01b39c3-6e14-4a5c-a7c9-52a79f6793d5',1,3,'NULL','2770','2024-09-18 11:33:36','2024-09-18 11:33:36'),(105,'N9830','N9830','高健','c1151862-8e5c-4c76-8f14-41ec6de8382f',1,7,'NULL','N9830','2024-09-18 11:33:36','2024-09-18 11:33:36'),(106,'N1310','N1310','5B病房','c327e180-8b33-4158-a3d2-df764f9a98a5',1,5,'NULL','N1310','2024-09-18 11:33:36','2024-09-18 11:33:36'),(107,'M140','M140','公關室','c7a11fd5-9f8f-4d1d-ab79-ff10f40595db',1,5,'NULL','M140','2024-09-18 11:33:36','2024-09-18 11:33:36'),(108,'05007','05007','張瑋真','cdbc251d-fa60-4ae0-95a8-9848fb2e8c94',1,5,NULL,'05007','2024-09-18 11:33:36','2024-09-18 11:33:36'),(109,'05043','05043','林楷哲','cf6a2737-cb74-4b7e-be8a-cd3d89986d27',1,5,NULL,'05043','2024-09-18 11:33:36','2024-09-18 11:33:36'),(110,'06337','06337','林彥慈','d20b01e6-801f-4678-85f1-a019d8f8d05e',1,9,NULL,'06337','2024-09-18 11:33:36','2024-09-18 11:33:36'),(111,'3000','3000','陳碧茹','d2d9700d-fc90-4565-8d6a-dc9987113bc3',1,3,'NULL','3000','2024-09-18 11:33:36','2024-09-18 11:33:36'),(112,'CHOT01','CHOT01','金萬林管理員','d359f02b-0a85-405e-91fe-71c8fdcba215',1,3,'NULL','CHOT01','2024-09-18 11:33:36','2024-09-18 11:33:36'),(113,'ping1340','23028','黃婉萍','d35a2be9-4417-4639-83e1-d2629ab53fde',1,1,NULL,'ping1340','2024-09-18 11:33:36','2024-09-18 11:33:36'),(114,'03512','03512','江毓鳳','d3638363-df30-4e2e-bb99-d8e3eb95ef04',1,5,NULL,'03512','2024-09-18 11:33:36','2024-09-18 11:33:36'),(115,'N9200','N9200','放射科','d4b63210-04e0-4c14-bef1-7e9e69e6b764',1,5,'NULL','N9200','2024-09-18 11:33:36','2024-09-18 11:33:36'),(116,'04804','04804','張瓊丹','e28831d2-a1fb-46f6-816d-7fcc13ea159d',1,3,NULL,'04804','2024-09-18 11:33:36','2024-09-18 11:33:36'),(117,'3860','3860','楊智超','e596658d-7820-4862-94fb-daf2245474f1',1,3,'NULL','3860','2024-09-18 11:33:36','2024-09-18 11:33:36'),(118,'M8163','M8163','8A病房','e66707b7-9643-4db5-93fa-dc1ce6cd1ced',1,5,'NULL','M8163','2024-09-18 11:33:36','2024-09-18 11:33:36'),(119,'wfjh10602@gmail.com','123456','lin','e81e9b00-b8c6-498d-9cda-be4b62ffe0e2',1,1,NULL,'wfjh10602@gmail.com','2024-09-18 11:33:36','2024-09-18 11:33:36'),(120,'N1340','N1340','5A病房','ea18ea52-bbf4-4cf0-892a-c5133aab890c',1,5,'NULL','N1340','2024-09-18 11:33:36','2024-09-18 11:33:36'),(121,'05626','05626','黃湘庭','eafe7dcf-0f17-471b-90b7-584ecb34d10d',1,5,NULL,'05626','2024-09-18 11:33:36','2024-09-18 11:33:36'),(122,'3634','3634','莊汭潸','ec44165e-968c-432c-91c1-3333024749a5',1,3,'NULL','3634','2024-09-18 11:33:36','2024-09-18 11:33:36'),(123,'N2320','N2320','6B病房','ec876ab3-31a5-4f26-b43f-8a35c760594a',1,5,'NULL','N2320','2024-09-18 11:33:36','2024-09-18 11:33:36'),(124,'DCL','0513','金萬林管理員','ed239c66-1f72-4a7e-a5c9-7a6481b21a3a',1,1,NULL,'DCL','2024-09-18 11:33:36','2024-09-18 11:33:36'),(125,'kun','kun','kun','ed239c66-1f72-4a7e-a5c9-7a6481b21a3b',1,1,'NULL','kun','2024-09-18 11:33:36','2024-09-18 11:33:36'),(126,'3179','3179','劉佳其','f0a8170c-0fae-49c4-94bc-2aedbf2a7b6f',1,3,'NULL','3179','2024-09-18 11:33:36','2024-09-18 11:33:36'),(127,'N22C0','N22C0','牙科','f15e410f-dc23-4077-aae2-620a7b4c67bf',1,5,'NULL','N22C0','2024-09-18 11:33:36','2024-09-18 11:33:36'),(128,'N3310','N3310','RCC','f4a6b885-8080-4bd7-840c-f37ca1a7c351',1,5,'NULL','N3310','2024-09-18 11:33:36','2024-09-18 11:33:36'),(129,'cookie','23012','楊于萱','fd36c6e5-f713-4c13-836e-f2d3e7a051b5',1,1,NULL,'cookie','2024-09-18 11:33:36','2024-09-18 11:33:36');
/*!40000 ALTER TABLE `member` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `my_announcement`
--

DROP TABLE IF EXISTS `my_announcement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `my_announcement` (
  `id` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(1000) DEFAULT NULL,
  `Content` varchar(2000) DEFAULT NULL,
  `BeginPublishTime` datetime DEFAULT NULL,
  `EndPublishTime` datetime DEFAULT NULL,
  `BeginViewTime` datetime DEFAULT NULL,
  `EndViewTime` datetime DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `AnnounceID` varchar(100) NOT NULL,
  `CreatorID` varchar(100) DEFAULT NULL COMMENT '對準 Member 的 UserID',
  `UserID` varchar(100) DEFAULT NULL,
  `IsBookToTop` tinyint(1) NOT NULL DEFAULT '0',
  `IsRemind` tinyint(1) NOT NULL DEFAULT '0',
  `CreatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedTime` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `unique_announceId_userId` (`AnnounceID`,`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=109 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='我的公告';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `my_announcement`
--

LOCK TABLES `my_announcement` WRITE;
/*!40000 ALTER TABLE `my_announcement` DISABLE KEYS */;
/*!40000 ALTER TABLE `my_announcement` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-09-18 19:37:48
