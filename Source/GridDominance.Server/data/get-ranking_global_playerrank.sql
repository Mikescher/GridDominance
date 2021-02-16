SELECT
	usr.userid,
	usr.username,
	usr.score as totalscore,
	usr.time_total as totaltime,
	(
		SELECT COUNT(*)  + 1
		FROM users othr
		WHERE
		      (othr.score > usr.score) OR
		      (othr.score = usr.score AND othr.time_total < usr.time_total ) OR
		      (othr.score = usr.score AND othr.time_total = usr.time_total AND othr.userid < usr.userid )
	) AS "rank"

FROM users usr

WHERE usr.userid = :uid
