## My sql databas setup

CREATE DATABASE bookngo;
CREATE USER 'bookngo'@'localhost' IDENTIFIED BY 'bookngo';
GRANT ALL PRIVILEGES ON bookngo.\* TO 'bookngo'@'localhost';

## Posts

http://localhost:5218/users

{
"name": "Azar",
"email": "Azar@email.com",
"password": "h3mligt"
}
