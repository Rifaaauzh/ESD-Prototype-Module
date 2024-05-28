
const float analogReading = A0;
void setup() {
  Serial.begin(115200);
  Serial.println("Arduino ready");
}

void loop() {
 float sensorValue = analogRead(analogReading);
 delay(100);
 float voltage = (sensorValue * 3.3) / 1023.0;
 delay(100);
  /*if (Serial.available() > 0) {
    String data = Serial.readString();
    delay(100);
    Serial.print("Received: ");
    Serial.println(data);
  }*/
  Serial.println(voltage);
  delay(2000);
}
