SELECT
  users.userid AS userid,
  users.username AS username,
  users.score_stars AS totalscore,
  users.time_total AS totaltime

FROM users

WHERE users.score_stars > 0

ORDER BY
  totalscore DESC,
  userid ASC

LIMIT :qlimit
OFFSET :qpage