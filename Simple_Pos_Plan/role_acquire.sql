DROP FUNCTION IF EXISTS role_acquire(integer, varchar);

CREATE FUNCTION role_acquire (
    p_role_id INT,
    p_search VARCHAR(100)
)
RETURNS TABLE(
    role_id INT,
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
        r.role_id,
        r.name,
        r.details,
        r.sort_order,
        r.stat_id,
        s.name AS stat_name
    FROM roles r
    LEFT JOIN stats s ON r.stat_id = s.stat_id
    WHERE r.stat_id <> 0
      AND (r.role_id = p_role_id OR p_role_id = 0)
      AND (
            r.name ILIKE '%' || p_search || '%'
         OR r.details ILIKE '%' || p_search || '%'
         OR p_search = ''
      );

END;
$function$;
