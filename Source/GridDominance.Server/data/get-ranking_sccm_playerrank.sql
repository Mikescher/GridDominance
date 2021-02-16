SELECT
	usr.userid,
	usr.username,
	usr.score_sccm as totalscore,
	usr.time_total as totaltime,
	(
		SELECT COUNT(*) + 1
		FROM users othr
		WHERE (othr.score_sccm > usr.score_sccm) OR (othr.score_sccm = usr.score_sccm AND othr.userid < usr.userid )
	) AS "rank"

FROM users usr

WHERE usr.userid = :uid
