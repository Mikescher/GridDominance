SELECT
  users.userid AS userid,
  users.username AS username,
  users.score AS totalscore,
  users.time_total AS totaltime

FROM users

WHERE score > 0

ORDER BY
  totalscore DESC,
  totaltime ASC,
  userid ASC

LIMIT :qlimit
OFFSET :qpage