# Welcome to the NDataAudit project!

[![Build Status](https://travis-ci.org/hectorsosajr/NDataAudit.svg?branch=master)](https://travis-ci.org/hectorsosajr/NDataAudit.svg?branch=master)

NDataAudit is a light-weight database auditing framework. It allows for the creation of audits in XML format. The framework sends the results of the audits as HTML emails.

It currently supports the following database engines:

|Logo|Database Name
|---|---
|[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/72848767/images/32_SQLServer.png)]()|Microsoft SQL Server
|[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/72848767/images/32_PostGres.png)]()|PostgreSQL
|[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/72848767/images/32_Redshift.png)]()|Amazon's RedShift
|[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/72848767/images/32_MySQL.png)]()|Oracle's MySQL
|[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/72848767/images/32_Sqlite.png)]()|SQLite
|[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/72848767/images/32_Hive.png)]()|Hadoop Hive

Researching:

|Logo|Database Name
|---|---
|[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/72848767/images/32_MongoDb.png)]()|MongoDB
|[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/72848767/images/32_RavenDb.png)]()|RavenDb
|[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/72848767/images/32_HBase.png)]()|HBase

## Output Styles

Output styles are defined in the TableTemplates.json file. This is just JSON arrays of CSS styles. Included in this file are the following templates:
* GreenReport
* Green
* GreenFancy
* RedReport
* Yellow
* YellowReport
* BlueReport
* BlueFancy
* RedShift - Imitates AWS Redshift page colors

Additional styles can be added easily by just adding another entry in the root JSON array in TableTemplates.json file. To use your new CSS template just add the name of your new template inside the \<template>\</template> tags.

### __Audit - Original - Blue Fancy CSS Template__
----
[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/cc47424d/images/Audit-BlueFancyTemplate.png)]()

### __Unit Test - Blue Fancy CSS Template__
----
[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/cc47424d/images/UnitTest-BlueFancyTemplate.png)]()

### __Unit Test - Green Fancy CSS Template__
----
[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/d7fa0ab7/images/UnitTest-GreenFancy%20Template.png)]()

### __Unit Test - RedShift CSS Template__
----
[![](https://cdn.rawgit.com/hectorsosajr/NDataAudit/d7fa0ab7/images/UnitTest-RedShift%20Template.png)]()