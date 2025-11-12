#!/bin/bash

# ============================================
# Script: test-kafka.sh
# Descripción: Script de validación de Kafka
# Autor: Nico (Persona 1 - DB & Infrastructure Lead)
# Uso: ./test-kafka.sh
# ============================================

set -e  # Salir si hay errores

# Colores para output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo "=========================================="
echo "🧪 TEST DE INFRAESTRUCTURA KAFKA"
echo "=========================================="
echo ""

# ============================================
# 1. Verificar que los contenedores estén corriendo
# ============================================
echo -e "${YELLOW}1. Verificando contenedores...${NC}"

if ! docker ps | grep -q "catalogo_zookeeper"; then
    echo -e "${RED}❌ Error: Zookeeper no está corriendo${NC}"
    echo "Ejecuta: docker-compose up -d zookeeper"
    exit 1
fi
echo -e "${GREEN}✅ Zookeeper está corriendo${NC}"

if ! docker ps | grep -q "catalogo_kafka"; then
    echo -e "${RED}❌ Error: Kafka no está corriendo${NC}"
    echo "Ejecuta: docker-compose up -d kafka"
    exit 1
fi
echo -e "${GREEN}✅ Kafka está corriendo${NC}"

if ! docker ps | grep -q "catalogo_kafka_ui"; then
    echo -e "${YELLOW}⚠️  Advertencia: Kafka UI no está corriendo${NC}"
    echo "Ejecuta: docker-compose up -d kafka-ui"
else
    echo -e "${GREEN}✅ Kafka UI está corriendo en http://localhost:8081${NC}"
fi

echo ""

# ============================================
# 2. Esperar a que Kafka esté listo
# ============================================
echo -e "${YELLOW}2. Esperando a que Kafka esté listo...${NC}"

MAX_RETRIES=30
RETRY_COUNT=0

while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    if docker exec catalogo_kafka kafka-topics --bootstrap-server localhost:9092 --list > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Kafka está listo y aceptando conexiones${NC}"
        break
    fi

    RETRY_COUNT=$((RETRY_COUNT+1))
    echo -n "."
    sleep 1

    if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
        echo ""
        echo -e "${RED}❌ Error: Kafka no respondió después de ${MAX_RETRIES} segundos${NC}"
        exit 1
    fi
done

echo ""

# ============================================
# 3. Listar topics existentes
# ============================================
echo -e "${YELLOW}3. Listando topics existentes...${NC}"

TOPICS=$(docker exec catalogo_kafka kafka-topics --bootstrap-server localhost:9092 --list)

if [ -z "$TOPICS" ]; then
    echo -e "${YELLOW}⚠️  No hay topics creados aún${NC}"
else
    echo -e "${GREEN}Topics encontrados:${NC}"
    echo "$TOPICS"
fi

echo ""

# ============================================
# 4. Crear topic de prueba si no existe
# ============================================
TOPIC_NAME="product-events"

echo -e "${YELLOW}4. Verificando topic '${TOPIC_NAME}'...${NC}"

if echo "$TOPICS" | grep -q "^${TOPIC_NAME}$"; then
    echo -e "${GREEN}✅ Topic '${TOPIC_NAME}' ya existe${NC}"
else
    echo -e "${YELLOW}Creando topic '${TOPIC_NAME}'...${NC}"
    docker exec catalogo_kafka kafka-topics \
        --create \
        --topic ${TOPIC_NAME} \
        --bootstrap-server localhost:9092 \
        --partitions 3 \
        --replication-factor 1
    echo -e "${GREEN}✅ Topic '${TOPIC_NAME}' creado exitosamente${NC}"
fi

echo ""

# ============================================
# 5. Describir el topic
# ============================================
echo -e "${YELLOW}5. Información del topic '${TOPIC_NAME}':${NC}"

docker exec catalogo_kafka kafka-topics \
    --describe \
    --topic ${TOPIC_NAME} \
    --bootstrap-server localhost:9092

echo ""

# ============================================
# 6. Test de Producer/Consumer
# ============================================
echo -e "${YELLOW}6. Probando Producer/Consumer...${NC}"

# Mensaje de prueba
TEST_MESSAGE="$(cat <<'EOF'
{
  "EventType": "ProductCreated",
  "ProductId": 999,
  "ProductName": "Test Product from Shell Script",
  "Price": 99.99,
  "Timestamp": "$(date -u +"%Y-%m-%dT%H:%M:%SZ")"
}
EOF
)"

# Enviar mensaje
echo -e "${YELLOW}Enviando mensaje de prueba...${NC}"
echo "$TEST_MESSAGE" | docker exec -i catalogo_kafka kafka-console-producer \
    --topic ${TOPIC_NAME} \
    --bootstrap-server localhost:9092

echo -e "${GREEN}✅ Mensaje enviado${NC}"

# Consumir último mensaje
echo -e "${YELLOW}Consumiendo último mensaje...${NC}"
CONSUMED=$(docker exec catalogo_kafka kafka-console-consumer \
    --topic ${TOPIC_NAME} \
    --bootstrap-server localhost:9092 \
    --from-beginning \
    --max-messages 1 \
    --timeout-ms 5000 2>/dev/null | tail -1)

if [ -n "$CONSUMED" ]; then
    echo -e "${GREEN}✅ Mensaje recibido:${NC}"
    echo "$CONSUMED"
else
    echo -e "${RED}❌ No se pudo consumir el mensaje${NC}"
    exit 1
fi

echo ""

# ============================================
# 7. Verificar grupos de consumidores
# ============================================
echo -e "${YELLOW}7. Grupos de consumidores:${NC}"

CONSUMER_GROUPS=$(docker exec catalogo_kafka kafka-consumer-groups \
    --bootstrap-server localhost:9092 \
    --list 2>/dev/null)

if [ -z "$CONSUMER_GROUPS" ]; then
    echo -e "${YELLOW}⚠️  No hay grupos de consumidores activos${NC}"
else
    echo -e "${GREEN}Grupos encontrados:${NC}"
    echo "$CONSUMER_GROUPS"
fi

echo ""

# ============================================
# 8. Resumen Final
# ============================================
echo "=========================================="
echo -e "${GREEN}✅ TODOS LOS TESTS PASARON${NC}"
echo "=========================================="
echo ""
echo "📊 Resumen:"
echo "  - Zookeeper: ✅ Corriendo"
echo "  - Kafka: ✅ Corriendo"
echo "  - Kafka UI: http://localhost:8081"
echo "  - Topic '${TOPIC_NAME}': ✅ Activo"
echo "  - Producer/Consumer: ✅ Funcionando"
echo ""
echo "🎯 Próximos pasos:"
echo "  1. Levantar DataTier: docker-compose up data-tier"
echo "  2. Levantar BusinessTier: docker-compose up business-tier"
echo "  3. Acceder a Kafka UI: http://localhost:8081"
echo ""
echo "=========================================="
