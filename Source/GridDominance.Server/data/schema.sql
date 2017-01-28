
DROP DATABASE IF EXISTS grid_dominance;
CREATE DATABASE IF NOT EXISTS grid_dominance;

USE grid_dominance;




DROP TABLE IF EXISTS users;
CREATE TABLE IF NOT EXISTS users
(
  userid                  int(11)      NOT NULL AUTO_INCREMENT,
  username                varchar(64)  NOT NULL,
  password_hash           char(128)    NOT NULL,
  is_auto_generated       bit(1)       NOT NULL,
  score                   int(11)      NOT NULL,
  creation_time           timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP,
  last_online             timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP,
  creation_device_name    varchar(128) NOT NULL,
  creation_device_version varchar(128) NOT NULL,

  PRIMARY KEY (userid),
  UNIQUE KEY username (username)
);




DROP TABLE IF EXISTS level_highscores;
CREATE TABLE IF NOT EXISTS level_highscores 
(
  userid        int(11)    NOT NULL,
  levelid       char(36)   NOT NULL,
  difficulty    tinyint(4) NOT NULL,
  best_time     int(11)    NOT NULL,
  last_changed  timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

  PRIMARY KEY (userid,levelid,difficulty)
);




DROP TABLE IF EXISTS cache_levels;
CREATE TABLE IF NOT EXISTS cache_levels
(
  levelid           char(36)   NOT NULL,
  difficulty        tinyint(4) NOT NULL,
  best_time         int(11)    NOT NULL,
  completion_count  int(11)    NOT NULL
);




DROP TABLE IF EXISTS error_log;
CREATE TABLE IF NOT EXISTS error_log
(
  error_id              int(11)       NOT NULL AUTO_INCREMENT,
  userid                int(11)       NOT NULL,
  screen_resolution     varchar(256)  NOT NULL,
  game_version          varchar(256)  NOT NULL,
  exception_id          varchar(256)  NOT NULL,
  exception_message     varchar(4096) NOT NULL,
  exception_stacktrace  varchar(4096) NOT NULL,
  timestamp             timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  additional_info       varchar(4096) NOT NULL,

  PRIMARY KEY (error_id)
);
