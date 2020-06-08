IF NOT EXISTS 
    (SELECT name  
     FROM master.sys.server_principals
     WHERE name = 'NHSD')
BEGIN
    CREATE LOGIN [LoginName] WITH PASSWORD = '$(NHSD_PASSWORD)'
END
