SELECT
  users.userid AS userid,
  users.username AS username,
  users.mpscore AS totalscore,
  users.time_total AS totaltime

FROM users

WHERE users.mpscore > 0

ORDER BY
  totalscore DESC,
  totaltime ASC,
  userid ASC

LIMIT :qlimit
OFFSET :qpage