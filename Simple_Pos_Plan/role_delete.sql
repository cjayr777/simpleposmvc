DROP FUNCTION IF EXISTS role_delete(integer, varchar);
CREATE FUNCTION role_delete (
    p_role_id INT,
    p_name VARCHAR(100)
)
RETURNS TEXT
LANGUAGE plpgsql
AS $$
DECLARE
    row_count INT;
BEGIN

    IF p_role_id IS NULL OR p_role_id <= 0 THEN
        RETURN 'Role ID must be greater than zero (0).';
    END IF;

    IF p_name IS NULL OR p_name = '' THEN
        RETURN 'Name is required.';
    END IF;


    SELECT COUNT(*) INTO row_count
    FROM roles
    WHERE role_id = p_role_id
    AND stat_id <> 0;

    IF row_count = 0 THEN
       RETURN 'Error: No Entry found for: ' || p_name;
    END IF;

    UPDATE roles
    SET
        stat_id = 0
    WHERE role_id = p_role_id;

    RETURN '';

    EXCEPTION
    WHEN OTHERS THEN
        RETURN
            'Error: Deleting Product Type failed.' ||
            E'\nSQLSTATE: ' || SQLSTATE ||
            E'\nMessage: ' || SQLERRM;

END;
$$;

