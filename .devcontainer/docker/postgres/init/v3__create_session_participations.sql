-- session_participations
CREATE TABLE IF NOT EXISTS session_participations (
    session_id uuid NOT NULL REFERENCES study_sessions(session_id) ON DELETE RESTRICT,
    user_id uuid NOT NULL REFERENCES users(user_id) ON DELETE RESTRICT,
    attendance_status varchar(16) NOT NULL CHECK (
        attendance_status IN ('Unanswered', 'Onsite', 'Remote', 'Absent')
    ),
    comment varchar(500),
    responded_at timestamptz,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (session_id, user_id)
);
-- index
CREATE INDEX IF NOT EXISTS idx_session_participations_user_id ON session_participations (user_id);
CREATE INDEX IF NOT EXISTS idx_session_participations_session_status ON session_participations (session_id, attendance_status);
-- comment
COMMENT ON TABLE session_participations IS '勉強会出欠情報';
COMMENT ON COLUMN session_participations.session_id IS '勉強会ID';
COMMENT ON COLUMN session_participations.user_id IS 'ユーザーID';
COMMENT ON COLUMN session_participations.attendance_status IS '参加ステータス';
COMMENT ON COLUMN session_participations.comment IS 'コメント';
COMMENT ON COLUMN session_participations.responded_at IS '回答日時';
COMMENT ON COLUMN session_participations.updated_at IS '更新日時';