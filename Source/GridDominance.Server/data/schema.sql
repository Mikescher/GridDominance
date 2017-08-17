
DROP DATABASE IF EXISTS grid_dominance;
CREATE DATABASE IF NOT EXISTS grid_dominance;

USE grid_dominance;




DROP TABLE IF EXISTS users;
CREATE TABLE IF NOT EXISTS users
(
  userid                  int(11)       NOT NULL AUTO_INCREMENT,
  username                varchar(64)   NOT NULL,
  password_hash           char(128)     NOT NULL,
  is_auto_generated       boolean       NOT NULL,

  score                   int(11)       NOT NULL DEFAULT 0,
  score_w1                int(11)       NOT NULL DEFAULT 0,
  score_w2                int(11)       NOT NULL DEFAULT 0,
  score_w3                int(11)       NOT NULL DEFAULT 0,
  score_w4                int(11)       NOT NULL DEFAULT 0,
  mpscore                 int(11)       NOT NULL DEFAULT 0,
  time_total              int(11)       NOT NULL DEFAULT 0,
  time_w1                 int(11)       NOT NULL DEFAULT 0,
  time_w2                 int(11)       NOT NULL DEFAULT 0,
  time_w3                 int(11)       NOT NULL DEFAULT 0,
  time_w4                 int(11)       NOT NULL DEFAULT 0,

  revision_id             int(11)       UNSIGNED NOT NULL DEFAULT 0,

  creation_time           timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP,

  last_online             timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  app_version             varchar(24)   NOT NULL,
  device_name             varchar(128)  NOT NULL,
  device_version          varchar(128)  NOT NULL,
  unlocked_worlds         varchar(1024) NOT NULL,
  device_resolution       varchar(24)   NOT NULL,

  ping_counter            int           UNSIGNED NOT NULL DEFAULT 0,

  PRIMARY KEY (userid)
);




DROP TABLE IF EXISTS level_highscores;
CREATE TABLE IF NOT EXISTS level_highscores 
(
  userid        int(11)    NOT NULL,
  levelid       char(38)   NOT NULL,
  difficulty    tinyint(4) NOT NULL,
  best_time     int(11)    NOT NULL,
  last_changed  timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

  PRIMARY KEY (userid,levelid,difficulty)
);




DROP TABLE IF EXISTS cache_levels;
CREATE TABLE IF NOT EXISTS cache_levels
(
  levelid           char(38)   NOT NULL,
  difficulty        tinyint(4) NOT NULL,
  best_time         int(11)    NOT NULL,
  best_userid       int(11)    NOT NULL,
  best_last_changed timestamp  NOT NULL,
  completion_count  int(11)    NOT NULL,

  PRIMARY KEY (levelid,difficulty)
);




DROP TABLE IF EXISTS error_log;
CREATE TABLE IF NOT EXISTS error_log
(
  error_id              int(11)       NOT NULL AUTO_INCREMENT,
  userid                int(11)       NOT NULL,
  password_verified     boolean       NOT NULL,
  screen_resolution     varchar(256)  NOT NULL,
  app_version           varchar(24)   NOT NULL,
  exception_id          varchar(256)  NOT NULL,
  exception_message     varchar(4096) NOT NULL,
  exception_stacktrace  varchar(8192) NOT NULL,
  timestamp             timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  additional_info       varchar(8192) NOT NULL,
  acknowledged          boolean       NOT NULL DEFAULT FALSE,

  PRIMARY KEY (error_id)
);