-- users
-- table
CREATE TABLE IF NOT EXISTS users (
    user_id uuid PRIMARY KEY,
    login_id varchar(64) NOT NULL UNIQUE,
    display_name varchar(100) NOT NULL,
    email varchar(254) NOT NULL UNIQUE,
    password_hash varchar(255) NOT NULL,
    role varchar(16) NOT NULL CHECK (role IN ('User', 'Admin')),
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP
);
-- index
CREATE INDEX IF NOT EXISTS idx_users_login_id ON users (login_id);
-- comment
COMMENT ON COLUMN users.user_id IS 'ユーザーID';
COMMENT ON COLUMN users.login_id IS 'ログインID';
COMMENT ON COLUMN users.display_name IS '氏名';
COMMENT ON COLUMN users.email IS 'メールアドレス';
COMMENT ON COLUMN users.password_hash IS 'パスワードハッシュ';
COMMENT ON COLUMN users.role IS 'ロール';
COMMENT ON COLUMN users.is_active IS '有効フラグ';
COMMENT ON COLUMN users.created_at IS '作成日時';
COMMENT ON COLUMN users.updated_at IS '更新日時';