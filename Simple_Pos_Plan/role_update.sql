DROP FUNCTION IF EXISTS role_update(integer, varchar, varchar, int);
CREATE FUNCTION role_update (
    p_role_id INT,
    p_name VARCHAR(100),
    p_details VARCHAR(255),
    p_sort_order INT
)
RETURNS TEXT
LANGUAGE plpgsql
AS $$
DECLARE
    row_count INT;
BEGIN
    p_name := TRIM(p_name);
    p_details := TRIM(COALESCE(p_details, ''));
    p_sort_order := COALESCE(p_sort_order, 0);

    IF p_role_id IS NULL OR p_role_id < 1 THEN
        RETURN 'Role ID is required.';
    END IF;

    IF p_name IS NULL OR p_name = '' THEN
        RETURN 'Name is required.';
    END IF;

    IF p_sort_order < 0 THEN
        RETURN 'Sort Order must be either zero(0) or a positive number.';
    END IF;

    SELECT COUNT(*) INTO row_count
    FROM roles
    WHERE name = p_name
    AND role_id <> p_role_id
    AND stat_id <> 0;

    IF row_count > 0 THEN
        RETURN 'Error: Duplicate entry found for name: ' || p_name;
    END IF;

    UPDATE roles
    SET
        name = p_name,
        details = p_details,
        sort_order = p_sort_order
    WHERE role_id = p_role_id
    AND stat_id <> 0;

    RETURN '';

    EXCEPTION
    WHEN OTHERS THEN
        RETURN
            'Error: Updating Product Type failed.' ||
            E'\nSQLSTATE: ' || SQLSTATE ||
            E'\nMessage: ' || SQLERRM;

END;
$$;
