import { Pool } from 'pg';
import dotenv from 'dotenv';

dotenv.config();

export class Database {
  private static instance: Pool;

  public static getPool(): Pool {
    if (!Database.instance) {
      Database.instance = new Pool({
        host: process.env.DB_HOST,
        port: Number(process.env.DB_PORT),
        user: process.env.DB_USER,
        password: process.env.DB_PASSWORD,
        database: process.env.DB_NAME,
      });

      Database.instance.on('connect', () => {
        console.log('🔌 Connected to PostgreSQL');
      });
      
      Database.instance.on('error', (err) => {
        console.error('Error in PostgreSQL Pool:', err);
      });
    }
    return Database.instance;
  }
}
