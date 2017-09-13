(
  SELECT
    i2.worldid AS worldid,
    COALESCE(SUM(gj2.best_time), 0) AS best_time

  FROM idmap AS i2


    LEFT JOIN
    (
      SELECT
        lh2.levelid,
        lh2.best_time,
        lh2.difficulty
      FROM level_highscores lh2

        INNER JOIN
        (
          SELECT
            lh1.levelid,
            MAX(lh1.difficulty) as difficulty
          FROM idmap AS i1

            INNER JOIN level_highscores AS lh1
              ON
                i1.levelid = lh1.levelid AND
                lh1.userid = :uid1

          GROUP BY lh1.levelid
        ) AS gj1

          ON
            lh2.levelid = gj1.levelid AND
            lh2.userid = :uid2 AND
            lh2.difficulty = gj1.difficulty
    ) AS gj2

      ON gj2.levelid = i2.levelid

  GROUP BY i2.worldid
)
UNION ALL
(
  SELECT
    '*' AS worldid,
    SUM(lh2.best_time) AS best_time
  FROM level_highscores lh2

    INNER JOIN
    (
      SELECT
        lh1.levelid,
        MAX(lh1.difficulty) as difficulty
      FROM idmap AS i1

        INNER JOIN level_highscores AS lh1
          ON
            i1.levelid = lh1.levelid AND
            lh1.userid = :uid3

      GROUP BY lh1.levelid
    ) AS gj1

      ON
        lh2.levelid = gj1.levelid AND
        lh2.userid = :uid4 AND
        lh2.difficulty = gj1.difficulty
)