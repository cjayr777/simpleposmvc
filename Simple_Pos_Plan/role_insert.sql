DROP FUNCTION IF EXISTS role_insert(varchar, varchar, integer);
CREATE OR REPLACE FUNCTION role_insert(
    p_name VARCHAR(100),
    p_details VARCHAR(255),
    p_sort_order INT
)
RETURNS TEXT
LANGUAGE plpgsql
AS $$
DECLARE
    row_count INT;
    not_deleted INT;
BEGIN

    p_name := TRIM(p_name);
    p_details := TRIM(COALESCE(p_details, ''));
    p_sort_order := COALESCE(p_sort_order, 0);
    not_deleted := 1;

    IF p_name IS NULL OR p_name = '' THEN
        RETURN 'Name is required.';
    END IF;

    IF p_sort_order < 0 THEN
        RETURN 'Sort Order must be either zero(0) or a positive number.';
    END IF;

    SELECT COUNT(*) INTO row_count
    FROM roles
    WHERE name = p_name
    AND stat_id <> 0;

    IF row_count > 0 THEN
        RETURN 'Error: Duplicate entry found for name: ' || p_name;
    END IF;

    INSERT INTO roles (
        name,
        details,
        sort_order,
        stat_id
    )
    VALUES (
        p_name,
        p_details,
        p_sort_order,
        not_deleted
    );

    RETURN '';

    EXCEPTION
    WHEN OTHERS THEN
        RETURN
            'Error: Adding Product Type failed.' ||
            E'\nSQLSTATE: ' || SQLSTATE ||
            E'\nMessage: ' || SQLERRM;

END;
$$;
