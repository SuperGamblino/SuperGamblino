DROP procedure IF EXISTS `get_top_users`;
CREATE PROCEDURE `get_top_users`()
BEGIN
    SELECT * FROM Users ORDER BY Credits DESC LIMIT 10;
END;