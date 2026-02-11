DROP FUNCTION IF EXISTS producttype_acquire(integer, varchar);

CREATE FUNCTION producttype_acquire (
    p_product_type_id INT,
    p_search VARCHAR(100)
)
RETURNS TABLE(
    product_type_id INT,
    name VARCHAR(100),
    details VARCHAR(255),
    sort_order INT,
    stat_id INT,
    stat_name VARCHAR
)
LANGUAGE plpgsql
AS $function$
BEGIN

    p_search := TRIM(COALESCE(p_search, ''));

    RETURN QUERY
    SELECT
        pt.product_type_id,
        pt.name,
        pt.details,
        pt.sort_order,
        pt.stat_id,
        s.name AS stat_name
    FROM product_types pt
    LEFT JOIN stats s ON pt.stat_id = s.stat_id
    WHERE pt.stat_id <> 0
      AND (pt.product_type_id = p_product_type_id OR p_product_type_id = 0)
      AND (
            pt.name ILIKE '%' || p_search || '%'
         OR pt.details ILIKE '%' || p_search || '%'
         OR p_search = ''
      );

END;
$function$;
