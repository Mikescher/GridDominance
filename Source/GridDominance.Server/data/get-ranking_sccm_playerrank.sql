SELECT o.rank, o.userid, o.username, o.totalscore, o.totaltime
FROM
  (
    SELECT @rownum:=@rownum+1 As rank, x.userid, x.username, x.totalscore, x.totaltime
    FROM
      (
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
      ) x

      JOIN (SELECT @rownum := 0) r
  ) o

WHERE o.userid = :uid
