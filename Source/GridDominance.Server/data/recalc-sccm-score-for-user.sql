SELECT
  max_diff AS diff,
  COUNT(max_diff) AS levelcount

FROM
  (
    SELECT GREATEST
           (
               (CASE WHEN d0_time IS NULL THEN -1 ELSE 0 END),
               (CASE WHEN d1_time IS NULL THEN -1 ELSE 1 END),
               (CASE WHEN d2_time IS NULL THEN -1 ELSE 2 END),
               (CASE WHEN d3_time IS NULL THEN -1 ELSE 3 END)
           )
           AS max_diff

    FROM userlevels_highscores

    WHERE userid=:uid
  )
  AS score_greatest

WHERE max_diff <> -1

GROUP BY max_diff