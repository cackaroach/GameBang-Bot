CREATE OR REPLACE FUNCTION user_bets_insert_function()
	RETURNS TRIGGER AS $user_bets_insert_trigger$
	DECLARE
	BEGIN
		INSERT INTO user_points VALUES(DEFAULT, new.user_id, -new.point, CONCAT('bet: ', new.id));
		RETURN new;
	END;
$user_bets_insert_trigger$ language plpgsql;

CREATE OR REPLACE FUNCTION user_bets_update_function()
	RETURNS TRIGGER AS $user_bets_update_trigger$
	DECLARE
	BEGIN
		DELETE FROM user_points WHERE reason = CONCAT('earned: ', old.id) AND user_id = old.user_id;
		IF(new.earned > 0) THEN
			INSERT INTO user_points VALUES(DEFAULT, new.user_id, new.earned, CONCAT('earned: ', new.id));
		END IF;
		
		IF(new.point <> old.point) THEN
			DELETE FROM user_points WHERE reason = CONCAT('bet: ', old.id) AND user_id = old.user_id;
			INSERT INTO user_points VALUES(DEFAULT, new.user_id, -new.point, CONCAT('bet: ', new.id));
		END IF;

		RETURN new;
	END;
$user_bets_update_trigger$ language plpgsql;

CREATE OR REPLACE FUNCTION user_bets_delete_function()
	RETURNS TRIGGER AS $user_bets_delete_trigger$
	DECLARE
	BEGIN
		DELETE FROM user_points WHERE reason = CONCAT('earned: ', new.id);
		DELETE FROM user_points WHERE reason = CONCAT('bet: ', new.id);
		RETURN new;
	END;
$user_bets_delete_trigger$ language plpgsql;

CREATE TRIGGER user_bets_insert_trigger
	AFTER INSERT
	ON user_bets
	FOR EACH ROW
	EXECUTE PROCEDURE user_bets_insert_function();

CREATE TRIGGER user_bets_update_trigger
	AFTER UPDATE
	ON user_bets
	FOR EACH ROW
	EXECUTE PROCEDURE user_bets_update_function();
	
CREATE TRIGGER user_bets_delete_trigger
	AFTER DELETE
	ON user_bets
	FOR EACH ROW
	EXECUTE PROCEDURE user_bets_delete_function();