import amqp, { Connection, Channel, ConsumeMessage, Options } from "amqplib";
import dotenv from "dotenv";

dotenv.config();

export class RabbitMQ {
  private connection!: Connection;
  private channel!: Channel;
  private readonly queue: string;
  private readonly url: string;

  constructor() {
    this.url = process.env.RABBITMQ_URL as string;
    this.queue = process.env.INGESTION_QUEUE_NAME as string;
  }

  // Connected and initializes the channel and queue
  public async init(): Promise<void> {
    let retries = 5;
    while (retries) {
      try {
        this.connection = await amqp.connect(this.url);
        this.channel = await this.connection.createChannel();

        await this.channel.assertQueue(this.queue, { durable: true });
        console.log(`Queue '${this.queue}' created with success.`);
        
        return;
      } catch (error) {
        console.error(`Error to connect on RabbitMQ, trying again... (${retries} remaining attempts)`);
        retries--;
        
        await new Promise((res) => setTimeout(res, 5000)); // Wait 5s before trying again
      }
    }

    console.error("Unable to connect to RabbitMQ.");
    process.exit(1);
  }

  // Posts a message to the queue
  public async publish(message: string, queue: string, options?: Options.Publish): Promise<void> {
    if (!this.channel) {
      throw new Error("Channel not initialized. Call init() first.");
    }

    this.channel.sendToQueue(queue, Buffer.from(message), { persistent: true, ...options });
    console.log(`Posted message: ${message}`);
  }

  // Creates a consumer that processes messages with the callback function
  public async consume(callback: (message: ConsumeMessage) => Promise<void>): Promise<void> {
    if (!this.channel) {
      throw new Error("Channel not initialized. Call init() first.");
    }

    await this.channel.consume(this.queue, async (message) => {
      if (message) {
        try {
          await callback(message);
          this.channel.ack(message);
        } catch (err) {
          console.error("Error processing message:", err);
          
          this.channel.nack(message, false, false);
        }
      }
    });

    console.log("Waiting for messages...");
  }

  // Closes the connection and the channel
  public async close(): Promise<void> {
    await this.channel.close();
    await this.connection.close();
    console.log("Connection to RabbitMQ closed.");
  }
}