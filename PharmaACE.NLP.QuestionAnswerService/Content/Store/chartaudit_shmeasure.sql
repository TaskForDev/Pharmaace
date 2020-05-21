CREATE TABLE `shmeasure` (
  `ID` int(11) NOT NULL,
  `Tumor` varchar(12) DEFAULT NULL,
  `Line` varchar(5) DEFAULT NULL,
  `Regimen` varchar(25) DEFAULT NULL,
  `Segment` varchar(255) DEFAULT NULL,
  `SubSegment` varchar(255) DEFAULT NULL,
  `Status` varchar(25) DEFAULT NULL,
  `Monthyear` datetime DEFAULT NULL,
  `Share` decimal(65,30) DEFAULT NULL,
  `ShareR2M` decimal(65,30) DEFAULT NULL,
  `ShareR3M` decimal(65,30) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `trmeasure` (
  `ID` int(11) NOT NULL,
  `Tumor` varchar(12) DEFAULT NULL,
  `Line` varchar(5) DEFAULT NULL,
  `Segment` varchar(255) DEFAULT NULL,
  `Monthyear` datetime DEFAULT NULL,
  `Share` decimal(65,30) DEFAULT NULL,
  `ShareR2M` decimal(65,30) DEFAULT NULL,
  `ShareR3M` decimal(65,30) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `itrmeasure` (
  `ID` int(11) NOT NULL,
  `Tumor` varchar(12) DEFAULT NULL,
  `Line` varchar(5) DEFAULT NULL,
  `Segment` varchar(255) DEFAULT NULL,
  `Status` varchar(25) DEFAULT NULL,
  `Monthyear` datetime DEFAULT NULL,
  `Share` decimal(65,30) DEFAULT NULL,
  `ShareR2M` decimal(65,30) DEFAULT NULL,
  `ShareR3M` decimal(65,30) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
