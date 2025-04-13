import pytest
import time
import pika
import json
import psycopg2
from psycopg2.extras import RealDictCursor

def test_person_flow():
    person_name = "Ciro"
    birthdate = "1999-08-16"
    
    person_queue = "ingestion.person"

    # Publica a mensagem no Rabbit
    connection = pika.BlockingConnection(pika.ConnectionParameters(
    host='rabbitmq',
    port=5672,
    virtual_host='/',
    credentials=pika.PlainCredentials('admin', 'admin')
))
    channel = connection.channel()
    channel.queue_declare(queue=person_queue, durable=True)

    message = json.dumps({"name": "Ciro", "birthdate":"1999-08-16"})
    channel.basic_publish(
        exchange='',
        routing_key=person_queue,
        body=message,
        properties=pika.BasicProperties(delivery_mode=2),
    )
    print(f"Posted message: {message}")
    connection.close()

    # Waiting .NET consumer to process
    time.sleep(5)

    # Query PostgreSQL and validate the calculated age
    conn = psycopg2.connect(
        host='postgres',
        database='mydb',
        user='admin',
        password='admin'
    )
    cur = conn.cursor(cursor_factory=RealDictCursor)
    cur.execute("SELECT * FROM persons WHERE name = %s", (person_name,))
    result = cur.fetchone()
    cur.close()
    conn.close()
    
    assert result is not None, "People not found"
    assert result['age'] == 25, "Age is wrong"
