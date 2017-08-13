SELECT
  level_highscores.userid AS userid,
  users.username AS username,
  users.mpscore AS totalscore,
  0 AS totaltime

FROM level_highscores

  INNER JOIN users ON users.userid = level_highscores.userid

WHERE score > 0

GROUP BY level_highscores.userid

ORDER BY
  totalscore DESC,
  totaltime ASC,
  userid ASC

LIMIT 100