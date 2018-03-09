SELECT o.rank, o.userid, o.username, o.totalscore, o.totaltime
FROM
  (
    SELECT @rownum:=@rownum+1 As rank, x.userid, x.username, x.totalscore, x.totaltime
    FROM
      (
        SELECT
          users.userid AS userid,
          users.username AS username,
          :__FIELD_SCORE__  AS totalscore,
          :__FIELD_TIME__ AS totaltime

        FROM users

        WHERE :__FIELD_SCORE__ > 0

        ORDER BY
          totalscore DESC,
          totaltime ASC,
          userid ASC
      ) x

      JOIN (SELECT @rownum := 0) r
  ) o

WHERE o.userid = :uid

