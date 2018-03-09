SELECT
  users.userid AS userid,
  users.username AS username,
  :__FIELD_SCORE__ AS totalscore,
  :__FIELD_TIME__ AS totaltime

FROM users

WHERE :__FIELD_SCORE__ > 0

ORDER BY
  totalscore DESC,
  totaltime ASC,
  userid ASC

LIMIT :qlimit
OFFSET :qpage