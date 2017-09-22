SELECT
  users.userid AS userid,
  users.username AS username,
  users.score_#$$FIELD$$ AS totalscore,
  users.time_#$$FIELD$$ AS totaltime

FROM users

WHERE users.score_#$$FIELD$$ > 0

ORDER BY
  totalscore DESC,
  totaltime ASC,
  userid ASC

LIMIT :qlimit
OFFSET :qpage