import { ConsumeMessage } from 'amqplib';
import { RabbitMQ } from '../infrastructure/rabbitMQ';
import { PersonRepository, Person } from '../repositories/personRepository';

export class PersonService {
  private repository: PersonRepository;
  private fillAgeQueue: string;

  constructor() {
    this.repository = new PersonRepository();
    this.repository.createTable();
    this.fillAgeQueue = process.env.FILL_AGE_QUEUE_NAME as string;
  }


  public async createPerson(message: ConsumeMessage, rabbit: RabbitMQ): Promise<void> {
    const person: Person = JSON.parse(message.content.toString());

    if (!person.name || !person.birthdate) {
      throw new Error('Invalid data for person creation');
    }

    const createdPerson = await this.repository.insert(person);

    await rabbit.publish(JSON.stringify(createdPerson), this.fillAgeQueue, {})
  }
}
