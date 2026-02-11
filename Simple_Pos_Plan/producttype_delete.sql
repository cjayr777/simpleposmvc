DROP FUNCTION IF EXISTS producttype_delete(integer, varchar);
CREATE FUNCTION producttype_delete (
    p_product_type_id INT,
    p_name VARCHAR(100)
)
RETURNS TEXT
LANGUAGE plpgsql
AS $$
DECLARE
    row_count INT;
BEGIN

    IF p_product_type_id IS NULL OR p_product_type_id <= 0 THEN
        RETURN 'Product Type ID must be greater than zero (0).';
    END IF;

    IF p_name IS NULL OR p_name = '' THEN
        RETURN 'Name is required.';
    END IF;


    SELECT COUNT(*) INTO row_count
    FROM product_types
    WHERE product_type_id = p_product_type_id
    AND stat_id <> 0;

    IF row_count = 0 THEN
       RETURN 'Error: No Entry found for: ' || p_name;
    END IF;

    UPDATE product_types
    SET
        stat_id = 0
    WHERE product_type_id = p_product_type_id;

    RETURN '';

    EXCEPTION
    WHEN OTHERS THEN
        RETURN
            'Error: Deleting Product Type failed.' ||
            E'\nSQLSTATE: ' || SQLSTATE ||
            E'\nMessage: ' || SQLERRM;

END;
$$;

