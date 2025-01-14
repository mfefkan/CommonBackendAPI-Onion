-- Read-only kullanıcı oluşturma
CREATE USER CommonBackendRead WITH PASSWORD 'admin123';
GRANT CONNECT ON DATABASE "CommonBackendDB" TO CommonBackendRead;
GRANT USAGE ON SCHEMA public TO CommonBackendRead;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO CommonBackendRead;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO CommonBackendRead;

-- Write kullanıcı oluşturma
CREATE USER CommonBackendWrite WITH PASSWORD 'admin123';
GRANT CONNECT ON DATABASE "CommonBackendDB" TO CommonBackendWrite;
GRANT USAGE ON SCHEMA public TO CommonBackendWrite;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO CommonBackendWrite;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO CommonBackendWrite;
