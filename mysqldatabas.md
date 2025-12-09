## My sql databas setup ------------------------------------------------------
CREATE DATABASE bookngo;
CREATE USER 'bookngo'@'localhost' IDENTIFIED BY 'bookngo';
GRANT ALL PRIVILEGES ON bookngo.\* TO 'bookngo'@'localhost';

## DELETE http://localhost:5218/db ------------------------------------------------------------------

## POST http://localhost:5218/users -----------------------------------------------------------------
{
"name": "Azar",
"email": "Azar@email.com",
"password": "h3mligt"
}

## POST http://localhost:5218/login -----------------------------------------------------------------
{
"email": "Azar@email.com",
"password": "h3mligt"
}
Returnar true om matchar databasen & ger dig en cookie 

## Headers Set-Cookie
## GET http://localhost:5218/me ---------------------------------------------------------------------
{
  "name": "Azar",
  "email": "Azar@email.com",
  "status": "Logged in with cookie"
}
returnar [401 Unautthorized] om man försöker logga in utan Cookie i header
