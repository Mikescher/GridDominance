SELECT o.rank, o.userid, o.username, o.totalscore, o.totaltime
FROM
  (
    SELECT @rownum:=@rownum+1 As rank, x.userid, x.username, x.totalscore, x.totaltime
    FROM
      (
        SELECT
          level_highscores.userid AS userid,
          users.username AS username,
          SUM(
              CASE level_highscores.difficulty
              WHEN 0 THEN 11
              WHEN 1 THEN 13
              WHEN 2 THEN 17
              WHEN 3 THEN 23
              ELSE 0
              END
          ) AS totalscore,
          SUM(level_highscores.best_time) AS totaltime

        FROM level_highscores

        INNER JOIN users ON users.userid = level_highscores.userid

        GROUP BY level_highscores.userid

        ORDER BY
          totalscore DESC,
          totaltime ASC,
          userid ASC
      ) x

      JOIN (SELECT @rownum := 0) r
  ) o

WHERE o.userid = :uid
