import { Pool } from 'pg';
import { Database } from '../infrastructure/database';

// Definition of the Person entity
export interface Person {
  id?: number;
  name: string;
  birthdate: Date;
  age?: number;
}

export class PersonRepository {
  private pool: Pool;

  constructor() {
    this.pool = Database.getPool();
  }

  // Creates the table if it does not exist
  public async createTable(): Promise<void> {
    const query = `
      CREATE TABLE IF NOT EXISTS persons (
        id SERIAL PRIMARY KEY,
        name VARCHAR(255) NOT NULL,
        birthdate DATE NOT NULL,
        age INT NULL
      );
    `;
    await this.pool.query(query);
    console.log('Persons table created or already existing.');
  }

  // Inserts a new record into the persons table
  public async insert(person: Person): Promise<Person> {
    const query = `
      INSERT INTO persons (name, birthdate, age)
      VALUES ($1, $2, $3)
      RETURNING *;
    `;
    const values = [person.name, person.birthdate, person.age];
    const result = await this.pool.query(query, values);
    return result.rows[0];
  }
}
