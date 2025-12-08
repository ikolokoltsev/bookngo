## My sql databas setup

CREATE DATABASE bookngo;

CREATE USER 'bookngo'@'localhost' IDENTIFIED BY 'bookngo';

GRANT ALL PRIVILEGES ON bookngo.\* TO 'bookngo'@'localhost';

FLUSH PRIVILEGES
// betyder att MySQL ska läsa om alla användare och rättigheter från sina interna tabeller så att ändringarna börjar gälla direkt.
