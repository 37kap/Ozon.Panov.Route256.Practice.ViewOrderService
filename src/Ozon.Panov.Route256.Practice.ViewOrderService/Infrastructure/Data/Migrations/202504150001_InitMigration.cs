using FluentMigrator;
using Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Migrator;

namespace Ozon.Panov.Route256.Practice.ViewOrderService.Infrastructure.Data.Migrations;

[Migration(202504150001)]
public class InitMigration : ShardSqlMigration
{
    protected override string GetUpSql(IServiceProvider services)
        => """
           CREATE TABLE orders (
                   order_id BIGINT PRIMARY KEY,
                   region_id BIGINT NOT NULL,
                   status INT NOT NULL,
                   customer_id BIGINT NOT NULL,
                   comment TEXT NOT NULL,
                   created_at TIMESTAMPTZ NOT NULL
               );

           SET search_path TO public;

           DO $$
               BEGIN
                   IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'order_v1') THEN
                       CREATE TYPE order_v1 as
                       (
                         order_id BIGINT,
                         region_id BIGINT,
                         status INT,
                         customer_id BIGINT,
                         comment TEXT,
                         created_at TIMESTAMPTZ
                       );
                   END IF;
               END
           $$;
           """;

    protected override string GetDownSql(IServiceProvider services)
        => """
           DROP TABLE IF EXISTS orders;

           SET search_path TO public;

           DROP TYPE IF EXISTS order_v1;
           """;
}