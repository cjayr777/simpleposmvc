
DROP TABLE IF EXISTS stats;
CREATE TABLE stats (
    stat_id SERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    details VARCHAR(255) DEFAULT ''
);

DROP TABLE IF EXISTS products;
CREATE TABLE products (
    product_id SERIAL PRIMARY KEY,
    product_code VARCHAR(100) NOT NULL,
    name VARCHAR(100) NOT NULL,
    details VARCHAR(255) DEFAULT '',
    price DECIMAL(18, 2) DEFAULT 0,
    product_type_id INT DEFAULT 0,
    sort_order INT DEFAULT 0,
    stat_id INT DEFAULT 0
);

// todo: product_images table
DROP TABLE IF EXISTS product_images;
CREATE TABLE products (
    product_image_id SERIAL PRIMARY KEY,
    product_id INT DEFAULT 0,
    name VARCHAR(100) NOT NULL,
    blob BYTEA NOT NULL,
);

DROP TABLE IF EXISTS product_types;
CREATE TABLE product_types (
    product_type_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    details VARCHAR(255) DEFAULT '',
    sort_order INT DEFAULT 0,
    stat_id INT DEFAULT 0
);

DROP TABLE IF EXISTS roles;
CREATE TABLE roles (
    role_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    details VARCHAR(255) DEFAULT '',
    sort_order INT DEFAULT 0,
    stat_id INT DEFAULT 0
);

DROP TABLE IF EXISTS transactions;
CREATE TABLE transactions (
    transaction_id SERIAL PRIMARY KEY,
    transaction_code VARCHAR(100) NOT NULL,
    transaction_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    seller_id INT DEFAULT 0,
    grand_total DECIMAL(18, 0) DEFAULT 0,
    paid_amount DECIMAL(18, 0) DEFAULT 0,
    change_amount DECIMAL(18, 0) DEFAULT 0,
    stat_id INT DEFAULT 0
);

DROP TABLE IF EXISTS purchases;
CREATE TABLE purchases (
    purchase_id SERIAL PRIMARY KEY,
    transaction_id INT DEFAULT 0,
    product_id INT DEFAULT 0,
    quantity INT DEFAULT 0,
    unit_price DECIMAL(18, 0) DEFAULT 0,
    sub_total DECIMAL(18, 0) DEFAULT 0,
    stat_id INT DEFAULT 0
);

DROP TABLE IF EXISTS sellers;
CREATE TABLE sellers (
    seller_id SERIAL PRIMARY KEY,
    seller_code VARCHAR(100) NOT NULL,
    password VARCHAR(100) DEFAULT '',
    last_name VARCHAR(100) DEFAULT '',
    first_name VARCHAR(100) DEFAULT '',
    middle_name VARCHAR(100) DEFAULT '',
    stat_id INT DEFAULT 0
);
