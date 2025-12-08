## My sql databas setup ------------------------------------------------------
CREATE DATABASE bookngo;
CREATE USER 'bookngo'@'localhost' IDENTIFIED BY 'bookngo';
GRANT ALL PRIVILEGES ON bookngo.\* TO 'bookngo'@'localhost';

## DELETE --------------------------------------------------------------------
http://localhost:5218/db


## POST ---------------------------------------------------------------------
http://localhost:5218/users
{
"name": "Azar",
"email": "Azar@email.com",
"password": "h3mligt"
}

http://localhost:5218/login
{
"email": "Azar@email.com",
"password": "h3mligt"
}

if(true)
{
      Set-Cookie .AspNetCore.Session=CfDJ8CXcZI..........
      ..................................................
      .......................
}
else
{
      return no Cookie
}
## GET -------------------------------------------------------------------------------

http://localhost:5218/login

if(Cookie == true)
{
      Set-Cookie .AspNetCore.Session=CfDJ8CXcZI..........
      ..................................................
      .......................
} return 

{
      "name": "Azar",
      "email": "Azar@email.com"
} 
else
{
      null
}
## ----------------------------------------------------------------------------------