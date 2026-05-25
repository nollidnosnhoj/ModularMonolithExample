SELECT 'CREATE DATABASE notes_db'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'notes_db')\gexec

SELECT 'CREATE DATABASE consults_db'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'consults_db')\gexec

SELECT 'CREATE DATABASE audit_log_db'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'audit_log_db')\gexec
