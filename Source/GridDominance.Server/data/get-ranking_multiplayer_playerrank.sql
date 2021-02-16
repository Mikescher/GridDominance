SELECT
	usr.userid,
	usr.username,
	usr.mpscore as totalscore,
	usr.time_total as totaltime,
	(
		SELECT COUNT(*) + 1
		FROM users othr
		WHERE (othr.mpscore > usr.mpscore) OR (othr.mpscore = usr.mpscore AND othr.userid < usr.userid )
	) AS "rank"

FROM users usr

WHERE usr.userid = :uid
