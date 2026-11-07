-- study_sessions
-- table
CREATE TABLE IF NOT EXISTS study_sessions (
    session_id uuid PRIMARY KEY,
    title varchar(200) NOT NULL,
    description text,
    starts_at timestamptz NOT NULL,
    ends_at timestamptz NOT NULL CHECK (ends_at > starts_at),
    format varchar(16) NOT NULL CHECK (format IN ('Hybrid', 'OnsiteOnly', 'RemoteOnly')),
    capacity_onsite integer NOT NULL CHECK (capacity_onsite >= 0),
    capacity_remote integer NOT NULL CHECK (capacity_remote >= 0),
    created_by_user_id uuid NOT NULL REFERENCES users(user_id) ON DELETE RESTRICT,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CHECK (
        (
            format = 'OnsiteOnly'
            AND capacity_onsite >= 0
            AND capacity_remote = 0
        )
        OR (
            format = 'RemoteOnly'
            AND capacity_onsite = 0
            AND capacity_remote >= 0
        )
        OR (
            format = 'Hybrid'
            AND capacity_onsite >= 0
            AND capacity_remote >= 0
        )
    )
);
-- index
CREATE INDEX IF NOT EXISTS idx_study_sessions_starts_at ON study_sessions (starts_at);
CREATE INDEX IF NOT EXISTS idx_study_sessions_title ON study_sessions (title);
-- comment
COMMENT ON TABLE study_sessions IS '勉強会情報';
COMMENT ON COLUMN study_sessions.session_id IS '勉強会ID';
COMMENT ON COLUMN study_sessions.title IS 'タイトル';
COMMENT ON COLUMN study_sessions.description IS '概要';
COMMENT ON COLUMN study_sessions.starts_at IS '開始日時';
COMMENT ON COLUMN study_sessions.ends_at IS '終了日時';
COMMENT ON COLUMN study_sessions.format IS '開催形式';
COMMENT ON COLUMN study_sessions.capacity_onsite IS '対面枠';
COMMENT ON COLUMN study_sessions.capacity_remote IS 'リモート枠';
COMMENT ON COLUMN study_sessions.created_by_user_id IS '主催者ユーザーID';
COMMENT ON COLUMN study_sessions.created_at IS '作成日時';
COMMENT ON COLUMN study_sessions.updated_at IS '更新日時';