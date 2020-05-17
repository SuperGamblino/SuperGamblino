DROP procedure IF EXISTS `give_user_exp`;
CREATE PROCEDURE `give_user_exp`(IN given_exp INT, IN cur_user_id BIGINT, OUT did_level_increase BOOL, OUT cur_exp_needed INT, OUT cur_exp INT)
BEGIN
    DECLARE current_exp_user INT DEFAULT 0;
    DECLARE current_level_user INT DEFAULT 0;
    DECLARE needed_exp INT DEFAULT 0;
    SELECT Level INTO current_level_user FROM Users WHERE Id = cur_user_id;
    SELECT current_level_user * LOG10(current_level_user) * 100 + 100 INTO needed_exp;
    SET cur_exp_needed = needed_exp;
    UPDATE Users SET Experience = Experience + given_exp WHERE Id = cur_user_id;
    SELECT Experience INTO cur_exp FROM Users WHERE Id = cur_user_id;
    IF cur_exp > needed_exp THEN
        SET did_level_increase = TRUE;
        SET cur_exp = cur_exp - needed_exp;
        UPDATE Users SET Experience = cur_exp, Level = Level + 1 WHERE Id = cur_user_id;
    ELSE
        SET did_level_increase = FALSE;
    END IF;
END;