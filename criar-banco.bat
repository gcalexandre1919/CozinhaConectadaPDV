@echo off
cd /D "C:\Users\GC\Documents\GitHub\CozinhaConectada\SistemaPDV"
sqlite3 "src\Infrastructure\SistemaPDV.Infrastructure\bin\Debug\net8.0\PDV.db" < "criar-tabelas.sql"
echo Tabelas criadas com sucesso!
pause
