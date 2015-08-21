1、操作系统及环境要求：Windows 7 以上，.net 4.5
2、数据库：oracle 11g,Microsoft Server SQL 2008 
4、进入AutoUpdateDataBin文件压 打开config文件：AutoUpdateData.exe.config，修改对应的数据库 连接。
   及 Contarct, PRIME_COMMODITY
5、检查数据库是否正常。
7、MSSQL 2008建立日记表：ilog,在 ../doc/sql/iLog.SQL 其它SQL为测试用的SQL。
6、直接运行：AutoUpdateData.exe



***********
运行后，当前目录有生成:
1、log文件夹：主要是软件运行的生成的log
2、TableUploadNum：主要是记录当天每个表上传的记录数。
3、Set.ini主要是记录设置相关。