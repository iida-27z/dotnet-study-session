-- Test data: 3 sessions
INSERT INTO study_sessions (
        session_id,
        title,
        description,
        starts_at,
        ends_at,
        format,
        capacity_onsite,
        capacity_remote,
        created_by_user_id,
        created_at,
        updated_at
    )
VALUES (
        'aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa',
        'SQL入門セミナー',
        '基本的なSELECT文と結合の考え方を学ぶセッションです。',
        NOW() + INTERVAL '7 days',
        NOW() + INTERVAL '8 days',
        'Hybrid',
        10,
        20,
        '11111111-1111-4111-8111-111111111111',
        NOW(),
        NOW()
    ),
    (
        'bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb',
        'テスト設計の基礎',
        '業務ロジックをテストする観点と設計方法を確認します。',
        NOW() + INTERVAL '10 days',
        NOW() + INTERVAL '11 days',
        'OnsiteOnly',
        30,
        0,
        '11111111-1111-4111-8111-111111111111',
        NOW(),
        NOW()
    ),
    (
        'cccccccc-cccc-4ccc-8ccc-cccccccccccc',
        'Dockerと開発環境整備',
        'コンテナを使った開発体験をまとめて確認します。',
        NOW() + INTERVAL '14 days',
        NOW() + INTERVAL '15 days',
        'RemoteOnly',
        0,
        25,
        '11111111-1111-4111-8111-111111111111',
        NOW(),
        NOW()
    );