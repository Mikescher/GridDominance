SELECT
  users.userid AS userid,
  users.username AS username,
  users.score_sccm AS totalscore,
  users.time_total AS totaltime

FROM users

WHERE users.score_sccm > 0

ORDER BY
  totalscore DESC,
  userid ASC

LIMIT :qlimit
OFFSET :qpage