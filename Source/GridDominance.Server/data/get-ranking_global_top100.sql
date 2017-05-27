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

LIMIT 100