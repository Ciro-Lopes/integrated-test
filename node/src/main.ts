import { RabbitMQ } from './infrastructure/rabbitMQ';
import { PersonService } from './services/personService';

async function runConsumer() {
  const service = new PersonService();

  const rabbit = new RabbitMQ();
  await rabbit.init();
  await rabbit.consume(async (message) => await service.createPerson(message, rabbit));
}

runConsumer().catch(console.error);